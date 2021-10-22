using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3.Model;
using File = FilesStorage.models.File;

namespace FilesStorage.Interfaces
{
    public interface IFilesStorage : IDisposable
    {
        Task SaveFileAsync(string key, FileStream stream);

        Task<File> GetFileAsync(string key);

        Task DeleteFileAsync(string key);

        Task<ListObjectsResponse> GetFilesAsync();
    }
}