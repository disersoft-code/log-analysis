using LogAnalysis.Models;
using LogAnalysis.Services;
using Serilog;
using System.Text;

namespace LogAnalysis
{
    public partial class frmMain : Form
    {
        private readonly AnalyzeProcessFilesService _analyzeService;
        public frmMain()
        {
            InitializeComponent();

            Log.Information("Application started.");

            ConfigureResultsGrid();

            _analyzeService = new AnalyzeProcessFilesService();
            _analyzeService.AnalyzeProcessFilesEvent += AnalyzeProcessFilesEvent;
        }

        private void btnSelectFiles_Click(object sender, EventArgs e)
        {
            try
            {
                Log.Information("Opening file selection dialog.");

                using var dialog = new OpenFileDialog
                {
                    Title = "Select one or more files",
                    Filter = "TXT files (*.txt)|*.txt",
                    Multiselect = true
                };

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Log.Information("Files selected: {Count}", dialog.FileNames.Length);

                    string[] files = dialog.FileNames;

                    listBoxFiles.Items.Clear();
                    listBoxFiles.Items.AddRange(files);

                    btnAnalyze.Enabled = files.Length != 0;

                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error selecting files");
                MessageBox.Show("Ocurrió un error al seleccionar los archivos. Revisa los logs para más detalles.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureResultsGrid()
        {
            dataGridView1.Columns.Clear();

            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            dataGridView1.Columns.Add("FileName", "Archivo");
            dataGridView1.Columns.Add("SerialNumber", "Número de serie");
            dataGridView1.Columns.Add("Version", "Versión");
            dataGridView1.Columns.Add("VersionFirstSeen", "Fecha primera aparición");
            dataGridView1.Columns.Add("VersionCount", "Veces que apareció");
            dataGridView1.Columns.Add("VersionLastSeen", "Fecha última aparición");
            dataGridView1.Columns.Add("ModeChangeErrorCount", "Error cambio de modo");
            dataGridView1.Columns.Add("DepositErrorCount", "Operaciones con error");
            dataGridView1.Columns.Add("StorageErrorCount", "Error de almacenaje");
            dataGridView1.Columns.Add("SuccessfulDepositsCount", "Depósitos realizados");
            dataGridView1.Columns.Add("StoringCount", "Contando almacenado");
            dataGridView1.Columns.Add("DifferentUsersCount", "Usuarios diferentes usados");
            dataGridView1.Columns.Add("CollectionsCount", "Recolecciones realizadas");
        }

        private async void btnAnalyze_Click(object sender, EventArgs e)
        {
            try
            {
                var files = listBoxFiles.Items.Cast<string>().ToList();

                this.Cursor = Cursors.WaitCursor;
                btnAnalyze.Enabled = false;
                btnSelectFiles.Enabled = false;

                progBarGeneral.Visible = true;
                progBarGeneral.Minimum = 0;
                progBarGeneral.Maximum = files.Count;
                progBarGeneral.Value = 0;
                lblLineNumber.Visible = true;
                lblCurrentFile.Visible = true;
                dataGridView1.Rows.Clear();

                Log.Information("Starting analysis. Files count: {Count}", files.Count);

                await _analyzeService.AnalyzeFilesAsync(files);

                LoadResultsGrid(_analyzeService.AllRecords);

                MessageBox.Show("Proceso finalizado correctamente", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error exception");
                MessageBox.Show("Ocurrió un error durante el análisis. Revisa los logs para más detalles.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Log.Information("Analysis process ended, re-enabling buttons.");
                this.Cursor = Cursors.Default;
                btnAnalyze.Enabled = true;
                btnSelectFiles.Enabled = true;
                btnExportToExcel.Enabled = _analyzeService.AllRecords.Count > 0;
            }
        }

        private void AnalyzeProcessFilesEvent(object? sender, Models.AnalyzeProcessFilesEventArgs<Models.ProgressInformation> e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => AnalyzeProcessFilesEvent(sender, e)));
                return;
            }

            switch (e.Type)
            {
                case AnalyzeMessageType.Info:
                    break;
                case AnalyzeMessageType.Warning:
                    break;
                case AnalyzeMessageType.Error:
                    break;
                case AnalyzeMessageType.Progress:
                    progBarGeneral.Value = e.Data.CurrentProgress;
                    break;
                case AnalyzeMessageType.LineNumber:
                    lblLineNumber.Text = $"# línea: {e.Data.LineNumber}";
                    break;
                case AnalyzeMessageType.CurrentFile:
                    lblCurrentFile.Text = $"Procesando archivo: {e.Data.CurrentFile}";
                    break;
                default:
                    break;
            }
        }

        private void LoadResultsGrid(List<RecordAnalysis> records)
        {
            try
            {
                dataGridView1.Rows.Clear();

                foreach (var record in records)
                {
                    dataGridView1.Rows.Add(
                        record.FileName,
                        record.SerialNumber,
                        record.Version,
                        record.VersionFirstSeen == default ? "" : record.VersionFirstSeen.ToString("yyyy-MM-dd HH:mm:ss"),
                        record.VersionCount,
                        record.VersionLastSeen == default ? "" : record.VersionLastSeen.ToString("yyyy-MM-dd HH:mm:ss"),
                        record.ModeChangeErrorCount,
                        record.DepositErrorCount,
                        record.StorageErrorCount,
                        record.SuccessfulDepositsCount,
                        record.StoringCount,
                        record.Users.Count,
                        record.CollectionsCount
                    );
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error loading results into grid");
            }
        }



        public void ExportToCsv(DataGridView grid, string filePath)
        {
            var sb = new StringBuilder();

            // Headers
            var headers = grid.Columns
                .Cast<DataGridViewColumn>()
                .Select(c => $"\"{c.HeaderText}\"");

            sb.AppendLine(string.Join(",", headers));

            // Rows
            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.IsNewRow) continue;

                var cells = row.Cells
                    .Cast<DataGridViewCell>()
                    .Select(c => $"\"{c.Value?.ToString()?.Replace("\"", "\"\"")}\"");

                sb.AppendLine(string.Join(",", cells));
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        private void btnExportToExcel_Click(object sender, EventArgs e)
        {
            try
            {
                using var dialog = new SaveFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv",
                    FileName = "resultados.csv"
                };

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    ExportToCsv(dataGridView1, dialog.FileName);
                    MessageBox.Show("Exportado correctamente", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error exporting to CSV");
                MessageBox.Show("Ocurrió un error durante la exportación. Revisa los logs para más detalles.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
