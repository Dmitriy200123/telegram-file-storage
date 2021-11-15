using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Models;

namespace FileStorageApp.Data.InfoStorage.Storages.Files
{
    public interface IFilesStorage : IDisposable, IInfoStorage<File>
    {
        public Task<List<File>> GetByFilePropertiesAsync(Expression<Func<File, bool>> expression);
        public Task<List<File>> GetByFileNameSubstringAsync(string subString);
        public Task<List<File>> GetAllAsync();
        public new Task<File> GetByIdAsync(Guid id);
    }
}