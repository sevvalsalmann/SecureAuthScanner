using MinimalApiProject.Models;


using System.Diagnostics;

public class LocalCodeRepository : ICodeRepository
{
    private readonly List<IFormFile> _files;
    public LocalCodeRepository(List<IFormFile> files)
    {
        _files = files;
    }

    public async Task<IEnumerable<SourceFile>> GetAllCSFilesAsync()
    {
        var result = new List<SourceFile>();
        foreach (var file in _files)
        {
            using var reader = new StreamReader(file.OpenReadStream());
            string content = await reader.ReadToEndAsync();
            result.Add(new SourceFile { FilePath = file.FileName, Content = content });
        }
        return result;
    }

}
