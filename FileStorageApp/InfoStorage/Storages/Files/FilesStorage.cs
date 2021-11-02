using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Config;
using FileStorageApp.Data.InfoStorage.Models;
using Microsoft.EntityFrameworkCore;

namespace FileStorageApp.Data.InfoStorage.Storages.Files
{
    internal class FilesStorage : BaseStorage<File>, IFilesStorage
    {
        internal FilesStorage(IDataBaseConfig dataBaseConfig) : base(dataBaseConfig)
        {
        }

        public new async Task<List<File>> GetAllAsync()
        {
            var list = await base.GetAllAsync();
            return list
                .OrderByDescending(x => x.UploadDate)
                .ThenBy(x => x.Id)
                .ToList();
        }

        public Task<List<File>> GetByFilePropertiesAsync(Expression<Func<File, bool>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));
            return DbSet
                .Where(expression)
                .OrderByDescending(x => x.UploadDate)
                .ThenBy(x => x.Id)
                .ToListAsync();
        }

        public Task<List<File>> GetByFileNameSubstringAsync(string subString)
        {
            if (subString == null)
                throw new ArgumentNullException(nameof(subString));
            return DbSet
                .Where(x => x.Name.Contains(subString))
                .OrderByDescending(x => x.UploadDate)
                .ThenBy(x => x.Id)
                .ToListAsync();
        }
    }
}