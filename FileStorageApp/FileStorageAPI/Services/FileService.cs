using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileStorageAPI.Converters;
using FileStorageAPI.Models;
using FileStorageApp.Data.InfoStorage.Factories;
using FileStorageApp.Data.InfoStorage.Storages.Files;

namespace FileStorageAPI.Services
{
    public class FileService : IFileService
    {
        private readonly IFilesStorage filesStorage;
        private readonly IFileConverter fileConverter;

        public FileService(IInfoStorageFactory infoStorageFactory, IFileConverter fileConverter)
        {
            this.fileConverter = fileConverter;
            filesStorage = infoStorageFactory.CreateFileStorage();
        }

        public async Task<List<File>> GetFiles()
        {
            var filesFromDataBase = await filesStorage.GetAllAsync();
            return filesFromDataBase
                .Select(fileConverter.ConvertFile)
                .ToList();
        }

        public async Task<File> GetFileById(Guid id)
        {
            var file = await filesStorage.GetByIdAsync(id);
            return file is null ? null : fileConverter.ConvertFile(file);
        }

        public async Task<File> CreateFile(UploadFile uploadFile)
        {
            return new File();
        }

        public async Task<File> UpdateFile(UpdateFile model)
        {
            return new File();
        }

        public Task DeleteFile(Guid id)
        {
            return Task.FromResult("");
        }
    }
}