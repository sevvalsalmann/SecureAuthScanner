using MinimalApiProject.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class AuthorizationAnalyzer
{
    public List<ScanResult> AnalyzeFiles(IEnumerable<SourceFile> csFiles)
    {
        var results = new List<ScanResult>();

        foreach (var file in csFiles)
        {
            var lines = file.Content.Split(new[] { "\r\n", "\n", "\r" }, System.StringSplitOptions.None);
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
                            FilePath = file.FilePath,
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
                            FilePath = file.FilePath,
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

    private string ExtractMethodName(string line)
    {
        var parts = line.Split('(')[0].Split(' ');
        return parts.LastOrDefault() ?? "";
    }
}
