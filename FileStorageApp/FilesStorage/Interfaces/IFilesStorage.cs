using System;
using System.IO;
using System.Threading.Tasks;
using File = FilesStorage.models.File;

namespace FilesStorage.Interfaces
{
    public interface IFilesStorage : IDisposable
    {
        Task SaveFileAsync(string key, FileStream stream);

        File GetFile(string key);

        Task DeleteFileAsync(string key);
    }
}