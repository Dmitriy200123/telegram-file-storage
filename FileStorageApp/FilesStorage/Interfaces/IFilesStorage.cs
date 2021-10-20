using System.IO;
using System.Threading.Tasks;
using File = FilesStorage.models.File;

namespace FilesStorage.Interfaces
{
    public interface IFilesStorage
    {
        Task SaveFileAsync(string key, FileStream stream);

        Task<File> GetFileAsync(string key);
    }
}