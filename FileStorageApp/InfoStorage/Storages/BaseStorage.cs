using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Config;
using FileStorageApp.Data.InfoStorage.Models;
using Microsoft.EntityFrameworkCore;

namespace FileStorageApp.Data.InfoStorage.Storages
{
    internal abstract class BaseStorage<T> : DbContext, IInfoStorage<T>
        where T : class, IModel
    {
        private readonly string _connectionLink;

        protected BaseStorage(IDataBaseConfig dataBaseConfig)
        {
            _connectionLink = dataBaseConfig.GetConnectionString();
            Database.EnsureCreated();
        }

        protected DbSet<T> DbSet { get; set; }

        public async Task<bool> AddAsync(T fileSender)
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

        public async Task<bool> UpdateAsync(T fileSender)
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
            var entity = await GetByIdAsync(id);
            DbSet.Remove(entity);
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

        public Task<List<T>> GetAllAsync()
        {
            return DbSet.ToListAsync();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await DbSet.FirstAsync(x => x.Id == id);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_connectionLink);
        }
    }
}