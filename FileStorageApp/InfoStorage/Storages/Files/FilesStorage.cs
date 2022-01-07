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

        public Task<List<File>> GetAllAsync(bool useInclude = false, int? skip = null, int? take = null)
        {
            var query = DbSet.AsQueryable();

            return AddOptionsInQuery(query, useInclude, skip, take)
                .OrderByDescending(x => x.UploadDate)
                .ThenBy(x => x.Id)
                .ToListAsync();
        }

        public Task<File> GetByIdAsync(Guid id, bool useInclude = false)
        {
            var query = DbSet.AsQueryable();

            return AddOptionsInQuery(query, useInclude).FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<List<File>> GetByFilePropertiesAsync(Expression<Func<File, bool>> expression, bool useInclude = false, int? skip = null, int? take = null)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            var query = DbSet.Where(expression);

            return AddOptionsInQuery(query, useInclude, skip, take)
                .OrderByDescending(x => x.UploadDate)
                .ThenBy(x => x.Id)
                .ToListAsync();
        }

        public Task<List<File>> GetByFileNameSubstringAsync(string subString, bool useInclude = false, int? skip = null, int? take = null)
        {
            if (subString == null)
                throw new ArgumentNullException(nameof(subString));

            var query = DbSet.Where(x => x.Name.Contains(subString));

            return AddOptionsInQuery(query, useInclude, skip, take)
                .OrderByDescending(x => x.UploadDate)
                .ThenBy(x => x.Id)
                .ToListAsync();
        }

        public async Task<int> GetFilesCountAsync(Expression<Func<File, bool>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            var query = DbSet.Where(expression);
            return await query.CountAsync();
        }

        public Task<List<string>> GetFileNamesAsync() => DbSet.Select(fileInfo => fileInfo.Name).ToListAsync();

        private static IQueryable<File> AddOptionsInQuery(IQueryable<File> query, bool useInclude = false, int? skip = null, int? take = null)
        {
            if (useInclude)
                query = query
                    .Include(x => x.Chat)
                    .Include(x => x.FileSender);

            if (skip != null)
                query = query.Skip(skip.Value);

            if (take != null)
                query = query.Take(take.Value);

            return query;
        }
    }
}