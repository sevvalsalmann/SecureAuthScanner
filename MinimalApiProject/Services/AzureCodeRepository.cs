using System.Diagnostics;
using MinimalApiProject.Models;

public class AzureCodeRepository : ICodeRepository
{
    private readonly string _organization;
    private readonly string _project;
    private readonly string _repo;
    private readonly string _pat;
    private readonly string _tempFolder;

    public AzureCodeRepository(string organization, string project, string repo, string pat)
    {
        _organization = organization;
        _project = project;
        _repo = repo;
        _pat = pat;
        _tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
    }

    public async Task<IEnumerable<SourceFile>> GetAllCSFilesAsync()
    {
        Directory.CreateDirectory(_tempFolder);

        string repoUrl = !string.IsNullOrEmpty(_pat)
            ? $"https://{_pat}@dev.azure.com/{_organization}/{_project}/_git/{_repo}"
            : $"https://dev.azure.com/{_organization}/{_project}/_git/{_repo}";

        var psi = new ProcessStartInfo
        {
            FileName = "git",
            Arguments = $"clone {repoUrl} .",
            WorkingDirectory = _tempFolder,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        using var process = Process.Start(psi);
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            string error = await process.StandardError.ReadToEndAsync();
            throw new Exception($"Git clone failed: {error}");
        }

        var filePaths = Directory.GetFiles(_tempFolder, "*.cs", SearchOption.AllDirectories);
        var result = new List<SourceFile>();
        foreach (var path in filePaths)
        {
            string content = await File.ReadAllTextAsync(path);
            result.Add(new SourceFile
            {
                FilePath = path,
                Content = content
            });
        }
        return result;
    }
}
