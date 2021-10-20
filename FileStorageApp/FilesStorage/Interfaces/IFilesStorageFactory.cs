using System.Threading.Tasks;

namespace FilesStorage.Interfaces
{
    public interface IFilesStorageFactory
    {
        IS3FilesStorageOptions Options { get; }
        Task<IFilesStorage> CreateAsync();
    }
}