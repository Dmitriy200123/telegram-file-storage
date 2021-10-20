using System.Threading.Tasks;

namespace FilesStorage.Interfaces
{
    public interface IFilesStorageFactory
    {
        Task<IFilesStorage> CreateAsync();
    }
}