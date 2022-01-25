using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using File = FilesStorage.models.File;

namespace FilesStorage.Interfaces
{
    public interface IFilesStorage : IDisposable
    {
        Task SaveFileAsync(string key, Stream stream);

        Task<File> GetFileAsync(string key, string fileName = null);

        Task<Stream> GetFileStreamAsync(string key);

        Task DeleteFileAsync(string key);

        Task<IEnumerable<File>> GetFilesAsync();
    }
}