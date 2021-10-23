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
            return DbSet
                .Where(x => x.FullName.Contains(subString))
                .OrderBy(x => x.FullName)
                .ThenBy(x => x.Id)
                .ToListAsync();
        }

        public Task<List<FileSender>> GetByTelegramNameSubstringAsync(string userName)
        {
            return DbSet
                .Where(x => x.TelegramUserName.Contains(userName))
                .OrderBy(x => x.FullName)
                .ThenBy(x => x.Id)
                .ToListAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileSender>(entity =>
            {
                entity.ToTable("Senders");
                entity.HasIndex(e => e.Id);
                entity.Property(e => e.FullName).HasMaxLength(255);
                entity.Property(e => e.TelegramUserName).HasMaxLength(255);
            });
        }
    }
}