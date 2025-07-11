using MinimalApiProject.Models;

namespace MinimalApiProject.Services;

public class ScanService
{
    public List<ScanResult> ScanProject(string projectPath)
    {
        // Basit örnek
        return new List<ScanResult>
        {
            new ScanResult { FileName = "Example.cs", Issue = "[Authorize] missing", LineNumber = 42 }
        };
    }
}