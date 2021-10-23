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

        public Task<List<FileSender>> GetAllAsync()
        {
            return DbSet
                .OrderBy(x => x.FullName)
                .ThenBy(x => x.Id)
                .ToListAsync();
        }

        public Task<FileSender> GetByIdAsync(Guid id)
        {
            return DbSet.FirstAsync(x => x.Id == id);
        }

        public Task<List<FileSender>> GetBySubstringAsync(string subString)
        {
            return DbSet.Where(x => x.FullName.Contains(subString)).ToListAsync();
        }

        public Task<List<FileSender>> GetByUserNameAsync(string userName)
        {
            return DbSet.Where(x => x.TelegramUserName.Contains(userName)).ToListAsync();
        }

        public async Task<bool> AddAsync(FileSender fileSender)
        {
            await DbSet.AddAsync(fileSender);
            try
            {
                await SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public async Task<bool> UpdateAsync(FileSender fileSender)
        {
            DbSet.Update(fileSender);
            try
            {
                await SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var fileSender = await GetByIdAsync(id);
            DbSet.Remove(fileSender);
            try
            {
                await SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public async Task<bool> ContainsAsync(Guid id)
        {
            var fileSender = await DbSet.FirstOrDefaultAsync(x => x.Id == id);
            return fileSender != null;
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