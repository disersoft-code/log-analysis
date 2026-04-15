using LogAnalysis.Models;
using LogAnalysis.Utils;
using Serilog;

namespace LogAnalysis.Services;

internal class AnalyzeProcessFilesService
{
    public event EventHandler<AnalyzeProcessFilesEventArgs<ProgressInformation>>? AnalyzeProcessFilesEvent;

    public List<RecordAnalysis> AllRecords { get; set; } = [];

    public async Task AnalyzeFilesAsync(List<string> fileList)
    {
        if (fileList.Count == 0)
        {
            Log.Information("No files to analyze.");
            return;
        }

        int index = 0;
        AllRecords.Clear();

        foreach (var file in fileList)
        {
            Log.Information("Analyzing file: {FileName}", file);

            try
            {
                if (!GlobalFunctions.IsValidFile(file))
                {
                    Log.Warning("Invalid file skipped: {FileName}", file);
                    continue;
                }

                await ProcessFileAsync(file);
            }
            finally
            {
                index++;
                SendProgressEvent(index);
            }
        }
    }


    private async Task ProcessFileAsync(string filePath)
    {
        List<RecordAnalysis> records = [];
        string fileName = Path.GetFileName(filePath);
        int lineNumber = 0;
        RecordAnalysis? currentRecord = null;

        SendCurrentFile(fileName);

        using var reader = new StreamReader(filePath);

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            lineNumber++;
            SendLineNumberEvent(lineNumber);

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var version = GlobalFunctions.TryExtractVersion(line);
            var serial = GlobalFunctions.TryExtractSerial(line);
            var date = GlobalFunctions.TryParseDate(line);


            if (currentRecord == null)
            {
                Log.Information("Creating new record for file: {FileName}", fileName);
                currentRecord = CreateNewRecord(fileName, "");
            }


            // Si llega una nueva versión distinta, cerrar y abrir nuevo registro
            if (string.IsNullOrWhiteSpace(version) == false)
            {
                if (string.IsNullOrWhiteSpace(currentRecord.Version) == false && string.Equals(currentRecord.Version, version, StringComparison.OrdinalIgnoreCase) == false)
                {
                    Log.Information("Version change detected in file: {FileName}. Current version: {CurrentVersion}, New version: {NewVersion}", fileName, currentRecord.Version, version);
                    AddCurrentRecordIfValid(records, currentRecord);
                    currentRecord = CreateNewRecord(fileName, currentRecord.SerialNumber);
                }

                currentRecord.Version = version;

                if (date.HasValue)
                {
                    if (currentRecord.VersionCount == 0)
                    {
                        currentRecord.VersionFirstSeen = date.Value;
                    }
                    currentRecord.VersionLastSeen = date.Value;
                }

                currentRecord.VersionCount++;
                Log.Information("Version {Version} seen in file: {FileName}. Total count for this version: {VersionCount}", version, fileName, currentRecord.VersionCount);
            }

            if (string.IsNullOrWhiteSpace(serial) == false)
            {
                if (string.IsNullOrWhiteSpace(currentRecord.SerialNumber))
                {
                    Log.Information("Serial number {SerialNumber} found in file: {FileName}", serial, fileName);
                    currentRecord.SerialNumber = serial;
                }
                else if (string.Equals(currentRecord.SerialNumber, serial, StringComparison.OrdinalIgnoreCase) == false)
                {
                    Log.Information("Serial number change detected in file: {FileName}. Current serial: {CurrentSerial}, New serial: {NewSerial}", fileName, currentRecord.SerialNumber, serial);
                    AddCurrentRecordIfValid(records, currentRecord);
                    currentRecord = CreateNewRecord(fileName, serial);
                    //currentRecord.SerialNumber = serial;
                }
            }

            if (GlobalFunctions.ContainsIgnoreAccents(line, "NO FUE POSIBLE PASAR A MODO DEPOSITO"))
            {
                currentRecord.ModeChangeErrorCount++;
            }

            if (GlobalFunctions.ContainsIgnoreAccents(line, "TERMINO DE DEPOSITO CON ERROR"))
                currentRecord.DepositErrorCount++;

            if (GlobalFunctions.ContainsIgnoreAccents(line, "Error durante el almacenaje"))
                currentRecord.StorageErrorCount++;

            if (GlobalFunctions.ContainsIgnoreAccents(line, "Deposito exitoso"))
                currentRecord.SuccessfulDepositsCount++;

            if (GlobalFunctions.ContainsIgnoreAccents(line, "Almacenaje"))
                currentRecord.StoringCount++;

            if (GlobalFunctions.ContainsIgnoreAccents(line, "Deposito enviado a CIRREON con exito"))
            {
                var user = GlobalFunctions.TryExtractUser(line);
                if (string.IsNullOrWhiteSpace(user) == false)
                {
                    currentRecord.Users.Add(user);
                }
            }

            if (GlobalFunctions.ContainsIgnoreAccents(line, "TERMINO DE RECOLECCION"))
                currentRecord.CollectionsCount++;

        }

        AddCurrentRecordIfValid(records, currentRecord);

        if (records.Count > 0)
        {
            Log.Information("Before merging duplicates, {RecordCount} records remain for file: {FileName}", records.Count, fileName);

            PrintRecords(records);

            records = MergeDuplicateRecords(records);

            Log.Information("After merging duplicates, {RecordCount} records remain for file: {FileName}", records.Count, fileName);

            PrintRecords(records);

            AllRecords.AddRange(records);
        }
    }


    private void PrintRecords(List<RecordAnalysis> records)
    {
        foreach (var record in records)
        {
            Log.Information("Record for file: {FileName}, Serial: {SerialNumber}, Version: {Version}, Version Count: {VersionCount}, Version Start: {VersionStart}, Version End:{VersionEnd} Mode Change Errors: {ModeChangeErrorCount}, Deposit Errors: {DepositErrorCount}, Storage Errors: {StorageErrorCount}, Successful Deposits: {SuccessfulDepositsCount}, Storing Count: {StoringCount}, Collections Count: {CollectionsCount}, Users: {Users}",
                record.FileName,
                record.SerialNumber,
                record.Version,
                record.VersionCount,
                record.VersionFirstSeen.ToString("yyyy-MM-dd HH:mm"),
                record.VersionLastSeen.ToString("yyyy-MM-dd HH:mm"),
                record.ModeChangeErrorCount,
                record.DepositErrorCount,
                record.StorageErrorCount,
                record.SuccessfulDepositsCount,
                record.StoringCount,
                record.CollectionsCount,
                record.Users.Count
            );
        }

    }

    private RecordAnalysis CreateNewRecord(string fileName, string serialNumber)
    {
        return new RecordAnalysis
        {
            FileName = fileName,
            SerialNumber = serialNumber,
        };
    }

    private void AddCurrentRecordIfValid(List<RecordAnalysis> records, RecordAnalysis? currentRecord)
    {
        if (currentRecord == null)
            return;

        bool hasData =
            !string.IsNullOrWhiteSpace(currentRecord.SerialNumber) ||
            !string.IsNullOrWhiteSpace(currentRecord.Version) ||
            currentRecord.VersionCount > 0 ||
            currentRecord.ModeChangeErrorCount > 0 ||
            currentRecord.DepositErrorCount > 0 ||
            currentRecord.StorageErrorCount > 0 ||
            currentRecord.SuccessfulDepositsCount > 0 ||
            currentRecord.StoringCount > 0 ||
            currentRecord.CollectionsCount > 0 ||
            currentRecord.Users.Count > 0;

        if (hasData)
            records.Add(currentRecord);
    }

    public List<RecordAnalysis> MergeDuplicateRecords(List<RecordAnalysis> records)
    {
        if (records == null || records.Count == 0)
            return new List<RecordAnalysis>();

        var merged = records
            .GroupBy(r => new
            {
                FileName = r.FileName?.Trim(),
                SerialNumber = r.SerialNumber?.Trim(),
                Version = r.Version?.Trim()
            })
            .Select(group =>
            {
                var first = group.First();

                var result = new RecordAnalysis
                {
                    FileName = first.FileName,
                    SerialNumber = first.SerialNumber,
                    Version = first.Version,
                    VersionFirstSeen = group.Min(x => x.VersionFirstSeen),
                    VersionLastSeen = group.Max(x => x.VersionLastSeen),
                    VersionCount = group.Sum(x => x.VersionCount),
                    ModeChangeErrorCount = group.Sum(x => x.ModeChangeErrorCount),
                    DepositErrorCount = group.Sum(x => x.DepositErrorCount),
                    StorageErrorCount = group.Sum(x => x.StorageErrorCount),
                    SuccessfulDepositsCount = group.Sum(x => x.SuccessfulDepositsCount),
                    StoringCount = group.Sum(x => x.StoringCount),
                    CollectionsCount = group.Sum(x => x.CollectionsCount)
                };

                foreach (var user in group.SelectMany(x => x.Users))
                    result.Users.Add(user);

                return result;
            })
            .ToList();

        return merged;
    }

    private void SendProgressEvent(int progressValue)
    {
        ProgressInformation progress = new ProgressInformation
        {
            CurrentProgress = progressValue
        };

        AnalyzeProcessFilesEvent?.Invoke(this, new AnalyzeProcessFilesEventArgs<ProgressInformation>(
            data: progress,
            type: AnalyzeMessageType.Progress,
            message: ""
        ));
    }

    private void SendLineNumberEvent(int lineNumber)
    {
        if (lineNumber % 500 == 0)
        {
            ProgressInformation progress = new ProgressInformation
            {
                LineNumber = lineNumber
            };

            AnalyzeProcessFilesEvent?.Invoke(this, new AnalyzeProcessFilesEventArgs<ProgressInformation>(
                data: progress,
                type: AnalyzeMessageType.LineNumber,
                message: ""
            ));
        }
    }

    private void SendCurrentFile(string currentFile)
    {
        ProgressInformation progress = new ProgressInformation
        {
            CurrentFile = currentFile
        };

        AnalyzeProcessFilesEvent?.Invoke(this, new AnalyzeProcessFilesEventArgs<ProgressInformation>(
            data: progress,
            type: AnalyzeMessageType.CurrentFile,
            message: ""
        ));
    }




}
