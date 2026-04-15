namespace LogAnalysis.Models;

public class RecordAnalysis
{
    public string FileName { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public DateTime VersionFirstSeen { get; set; }
    public int VersionCount { get; set; }
    public DateTime VersionLastSeen { get; set; }
    public int ModeChangeErrorCount { get; set; }
    public int DepositErrorCount { get; set; }
    public int StorageErrorCount { get; set; }
    public int SuccessfulDepositsCount { get; set; }
    public int StoringCount { get; set; }
    public HashSet<string> Users { get; } = new(StringComparer.OrdinalIgnoreCase);
    public int CollectionsCount { get; set; }
}
