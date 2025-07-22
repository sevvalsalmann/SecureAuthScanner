public class LocalCodeRepository : ICodeRepository
{
    private readonly string _directoryPath;
    public LocalCodeRepository(string directoryPath)
    {
        _directoryPath = directoryPath;
    }

    public Task<IEnumerable<string>> GetAllCSFilesAsync()
    {
        var files = Directory.GetFiles(_directoryPath, "*.cs", SearchOption.AllDirectories);
        return Task.FromResult(files.AsEnumerable());
    }
}
