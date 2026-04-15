namespace LogAnalysis.Models;

internal class ProgressInformation
{
    public int CurrentProgress { get; set; }
    public int LineNumber { get; set; }
    public string CurrentFile { get; set; } = string.Empty;

}
