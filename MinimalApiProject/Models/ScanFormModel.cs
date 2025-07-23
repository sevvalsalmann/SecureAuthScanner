namespace MinimalApiProject.Models
{
    public class ScanFormModel
    {
        public string Mode { get; set; } // "Remote" veya "Local"
                                         // Remote için:
        public string Organization { get; set; }
        public string Project { get; set; }
        public string Repository { get; set; }
        public string PersonalAccessToken { get; set; }
        // Local için:
        public string LocalPath { get; set; }
    }

}
