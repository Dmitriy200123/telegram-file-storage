using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Config;
using FileStorageApp.Data.InfoStorage.Models;
using Microsoft.EntityFrameworkCore;

namespace FileStorageApp.Data.InfoStorage.Storages.FileSenders
{
    internal class FileSenderStorage : BaseStorage<FileSender>, IFileSenderStorage
    {
        internal FileSenderStorage(IDataBaseConfig dataBaseConfig) : base(dataBaseConfig)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileSender>().HasAlternateKey(sender => sender.TelegramId);
        }

        public new async Task<List<FileSender>> GetAllAsync()
        {
            var list = await base.GetAllAsync();
            return list
                .OrderBy(x => x.FullName)
                .ThenBy(x => x.Id)
                .ToList();
        }

        public Task<List<FileSender>> GetBySenderNameSubstringAsync(string subString)
        {
            if (subString == null)
                throw new ArgumentNullException(nameof(subString));
            return DbSet
                .Where(x => x.FullName.Contains(subString))
                .OrderBy(x => x.FullName)
                .ThenBy(x => x.Id)
                .ToListAsync();
        }

        public Task<List<FileSender>> GetByTelegramNameSubstringAsync(string userName)
        {
            if (userName == null)
                throw new ArgumentNullException(nameof(userName));
            return DbSet
                .Where(x => x.TelegramUserName.Contains(userName))
                .OrderBy(x => x.FullName)
                .ThenBy(x => x.Id)
                .ToListAsync();
        }

        public Task<bool> ContainsByTelegramIdAsync(long id)
        {
            return DbSet.AnyAsync(sender => sender.TelegramId == id);
        }
    }
}