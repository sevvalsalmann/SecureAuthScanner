using System.Diagnostics;
using MinimalApiProject.Models;

namespace MinimalApiProject.Services;

public class ScanService
{
    public async Task<List<ScanResult>> ScanRepositoryAsync(ScanRequest request)
    {
        var results = new List<ScanResult>();

        if (!Directory.Exists(request.RepositoryPath))
        {
            results.Add(new ScanResult
            {
                IssueType = $"ERROR: Repository path not found: {request.RepositoryPath}"
            });
            return results;
        }

        var csFiles = Directory.GetFiles(request.RepositoryPath, "*.cs", SearchOption.AllDirectories);

        foreach (var file in csFiles)
        {
            var lines = await File.ReadAllLinesAsync(file);
            string className = "";

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();

                if (line.StartsWith("public class"))
                    className = line.Split(' ')[2];

                if (line.StartsWith("public") && line.Contains("(") && line.Contains(")"))
                {
                    string methodName = ExtractMethodName(line);

                    var recentAttributes = lines.Take(i).Reverse().Take(5).Select(l => l.Trim()).ToList();
                    string? authorizeLine = recentAttributes.FirstOrDefault(l => l.StartsWith("[Authorize"));
                    bool hasAuthorize = authorizeLine != null;
                    bool hasAllowAnonymous = recentAttributes.Any(l => l.StartsWith("[AllowAnonymous"));
                    bool hasEmptyAuthorize = hasAuthorize && !authorizeLine.Contains("Roles") && !authorizeLine.Contains("Policy");

                    if (!hasAuthorize && !hasAllowAnonymous)
                    {
                        results.Add(new ScanResult
                        {
                            FilePath = file,
                            ClassName = className,
                            MethodName = methodName,
                            LineNumber = i + 1,
                            IssueType = "No authorization attribute found (missing [Authorize] or [AllowAnonymous])"
                        });
                    }
                    else if (hasEmptyAuthorize)
                    {
                        results.Add(new ScanResult
                        {
                            FilePath = file,
                            ClassName = className,
                            MethodName = methodName,
                            LineNumber = i + 1,
                            IssueType = "[Authorize] attribute is present but lacks roles or policy"
                        });
                    }
                }
            }
        }

        return results;
    }

    public async Task<List<ScanResult>> ScanAzureRepositoryAsync(ScanAzureRequest request)
    {
        var results = new List<ScanResult>();

        string tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempFolder);

        // Token ile HTTPS bağlantısı kurulması
        string repoUrl;
        if (!string.IsNullOrEmpty(request.PersonalAccessToken))
        {
            repoUrl = $"https://{request.PersonalAccessToken}@dev.azure.com/{request.Organization}/{request.Project}/_git/{request.Repository}";
        }
        else
        {
            repoUrl = $"https://dev.azure.com/{request.Organization}/{request.Project}/_git/{request.Repository}";
        }

        var psi = new ProcessStartInfo
        {
            FileName = "git",
            Arguments = $"clone {repoUrl} .",
            WorkingDirectory = tempFolder,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        using var process = Process.Start(psi);
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            string error = await process.StandardError.ReadToEndAsync();
            results.Add(new ScanResult
            {
                IssueType = $"ERROR: Git clone failed: {error}"
            });
            return results;
        }

        var scanRequest = new ScanRequest { RepositoryPath = tempFolder };
        return await ScanRepositoryAsync(scanRequest);
    }

    private string ExtractMethodName(string line)
    {
        var parts = line.Split('(')[0].Split(' ');
        return parts.LastOrDefault() ?? "";
    }
}
