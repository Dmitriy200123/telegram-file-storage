using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FileStorageAPI.Models;
using File = FileStorageAPI.Models.File;

namespace FileStorageAPI.Services
{
    public interface IFilesService
    {
        Task<List<File>> GetFiles();
        Task<File> GetFileById(Guid id);
        Task<File> CreateFile(UploadFile model);
        Task<File> UpdateFile(UpdateFile model);
        Task DeleteFile(Guid id);
    }
}