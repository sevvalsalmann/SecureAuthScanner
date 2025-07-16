namespace MinimalApiProject.Models;

public class ScanResult
{
    public string FilePath { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string MethodName { get; set; } = string.Empty;
    public int LineNumber { get; set; }
    public string IssueType { get; set; } = string.Empty;
}
