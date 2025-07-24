using MinimalApiProject.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface ICodeRepository
{
    Task<IEnumerable<SourceFile>> GetAllCSFilesAsync();
}
