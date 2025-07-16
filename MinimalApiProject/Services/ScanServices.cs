using System.Text.RegularExpressions;
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
                    string methodName = line.Split(' ').Last().Split('(')[0];

                    // Yukarıdaki 5 satıra bak, Authorize veya AllowAnonymous var mı
                    bool hasAuthorize = lines.Take(i).Reverse().Take(5).Any(l => l.Trim().StartsWith("[Authorize"));
                    bool hasEmptyAuthorize = lines.Take(i).Reverse().Take(5).Any(l =>
                    {
                        string t = l.Trim();
                        return t == "[Authorize]" || t == "[Authorize()]" || t == "[Authorize(\"\")]";
                    });

                    bool hasAllowAnonymous = lines.Take(i).Reverse().Take(5).Any(l => l.Trim().StartsWith("[AllowAnonymous"));

                    if (!hasAuthorize && !hasAllowAnonymous)
                    {
                        results.Add(new ScanResult
                        {
                            FilePath = file,
                            ClassName = className,
                            MethodName = methodName,
                            LineNumber = i + 1,
                            IssueType = "Missing [Authorize] attribute"
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
                            IssueType = "Empty or invalid [Authorize] attribute"
                        });
                    }
                }
            }
        }

        return results;
    }


    private string ExtractMethodName(string line)
    {
        var parts = line.Split('(')[0].Split(' ');
        return parts.LastOrDefault() ?? "";
    }
}
