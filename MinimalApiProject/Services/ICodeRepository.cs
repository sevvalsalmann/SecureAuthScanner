public interface ICodeRepository
{
    Task<IEnumerable<string>> GetAllCSFilesAsync();
}
