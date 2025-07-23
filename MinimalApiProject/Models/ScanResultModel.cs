namespace MinimalApiProject.Models
{
    public class ScanResultItem
    {
        public string FilePath { get; set; }
        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public int LineNumber { get; set; }
        public string IssueType { get; set; }
        public string Annotation { get; set; } 
    }

    public class ScanResultModel
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public List<ScanResultItem> Results { get; set; }
    }
}
