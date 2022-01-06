using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Models;

namespace FileStorageApp.Data.InfoStorage.Storages.FileSenders
{
    public interface IFileSenderStorage : IDisposable, IInfoStorage<FileSender>
    {
        public Task<List<FileSender>> GetBySenderNameSubstringAsync(string subString);

        public Task<List<FileSender>> GetByTelegramNameSubstringAsync(string userName);

        public Task<bool> ContainsByTelegramIdAsync(long id);
        public Task<List<FileSender>> GetAllAsync();
        public new Task<FileSender> GetByIdAsync(Guid id);
        public Task<FileSender?> GetByTelegramIdAsync(long id);
    }
}