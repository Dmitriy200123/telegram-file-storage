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

        public async Task<bool> AddAsync(File fileSender)
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

        public async Task<bool> UpdateAsync(File fileSender)
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
            var file = await GetByIdAsync(id);
            DbSet.Remove(file);
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
            var file = await DbSet.FirstOrDefaultAsync(x => x.Id == id);
            return file != null;
        }

        public Task<List<File>> GetAllAsync()
        {
            return DbSet
                .OrderByDescending(x => x.UploadDate)
                .ThenBy(x => x.Id)
                .ToListAsync();
        }

        public Task<File> GetByIdAsync(Guid id)
        {
            return DbSet.FirstAsync(x => x.Id == id);
        }

        public Task<List<File>> GetByFilePropertiesAsync(Expression<Func<File, bool>> expression)
        {
            return DbSet.Where(expression)
                .OrderByDescending(x => x.UploadDate)
                .ThenBy(x => x.Id)
                .ToListAsync();
        }

        public Task<List<File>> GetBySubStringAsync(string subString)
        {
            return DbSet.Where(x => x.Name.Contains(subString)).ToListAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<File>(entity =>
            {
                entity.ToTable("File");
                entity.HasIndex(e => e.Id);
                entity.HasKey(e => new {e.SenderId, e.ChatId});
                entity.Property(e => e.Name).HasMaxLength(255);
                entity.Property(e => e.Extension).HasMaxLength(255);
                entity.Property(e => e.Type).HasMaxLength(255);
            });
        }
    }
}