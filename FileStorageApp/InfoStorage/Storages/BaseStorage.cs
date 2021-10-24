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

        public async Task<bool> AddAsync(T entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));
            
            await DbSet.AddAsync(entity);
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

        public async Task<bool> UpdateAsync(T entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));
            
            DbSet.Update(entity);
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
            if (entity is null)
                throw new ArgumentException($"There is no entity with ID {id} in the database");

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
            var entity = await DbSet.FindAsync(id);
            return entity is not null;
        }

        public Task<List<T>> GetAllAsync()
        {
            return DbSet.ToListAsync();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await DbSet.FindAsync(id);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_connectionLink);
        }
    }
}