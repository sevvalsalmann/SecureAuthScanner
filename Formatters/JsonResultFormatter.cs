using MinimalApiProject.Models;
using System.Text.Json;

namespace MinimalApiProject.Formatters;

public static class JsonResultFormatter
{
    public static string Format(List<ScanResult> results)
    {
        return JsonSerializer.Serialize(results, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }
}