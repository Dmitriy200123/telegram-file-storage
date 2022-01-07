using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Models;

namespace FileStorageApp.Data.InfoStorage.Storages.Files
{
    public interface IFilesStorage : IDisposable, IInfoStorage<File>
    {
        public Task<List<File>> GetByFilePropertiesAsync(Expression<Func<File, bool>> expression, bool useInclude = false, int? skip = null, int? take = null);

        public Task<List<File>> GetByFileNameSubstringAsync(string subString, bool useInclude = false, int? skip = null, int? take = null);

        public Task<List<File>> GetAllAsync(bool useInclude = false, int? skip = null, int? take = null);

        public Task<File> GetByIdAsync(Guid id, bool useInclude = false);

        public Task<int> GetFilesCountAsync(Expression<Func<File, bool>> expression);

        public Task<List<string>> GetFileNamesAsync();
    }
}