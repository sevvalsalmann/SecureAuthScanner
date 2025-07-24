using MinimalApiProject.Models;

namespace MinimalApiProject.Services;

public class ScanService
{
    private readonly AuthorizationAnalyzer _analyzer = new();

    public async Task<List<ScanResult>> ScanLocalAsync(List<IFormFile> localFiles)
    {
        var repository = new LocalCodeRepository(localFiles);
        var files = await repository.GetAllCSFilesAsync(); 
        return _analyzer.AnalyzeFiles(files); 
    }



    public async Task<List<ScanResult>> ScanAzureAsync(string org, string project, string repo, string pat)
    {
        var repository = new AzureCodeRepository(org, project, repo, pat);
        var files = await repository.GetAllCSFilesAsync();
        return _analyzer.AnalyzeFiles(files);
    }
}
