namespace MinimalApiProject.Models
{
    public class ScanFormModel
    {
        public string Mode { get; set; } 
                                         
        public string Organization { get; set; }
        public string Project { get; set; }
        public string Repository { get; set; }
        public string PersonalAccessToken { get; set; }
      
        public string LocalPath { get; set; }
    }

}
