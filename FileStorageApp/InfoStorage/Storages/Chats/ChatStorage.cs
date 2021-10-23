using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Config;
using FileStorageApp.Data.InfoStorage.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FileStorageApp.Data.InfoStorage.Storages.Chats
{
    internal class ChatStorage : BaseStorage<Chat>, IChatStorage
    {
        internal ChatStorage(IDataBaseConfig dataBaseConfig) : base(dataBaseConfig)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Chat>(entity =>
            {
                entity.ToTable("Chats");
                entity.HasIndex(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(255);
            });
        }

        public Task<List<Chat>> GetAll()
        {
            return DbSet
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .ToListAsync();
        }

        public Task<Chat> GetById(Guid id)
        {
            return DbSet.FirstAsync(x => x.Id == id);
        }

        public Task<List<Chat>> GetBySubString(string subString)
        {
            return DbSet.Where(x => x.Name.Contains(subString)).ToListAsync();
        }

        public async Task<bool> Add(Chat chat)
        {
            await DbSet.AddAsync(chat);
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

        public async Task<bool> Update(Chat chat)
        {
            DbSet.Update(chat);
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

        public async Task<bool> Delete(Guid id)
        {
            var chat = await GetById(id);
            DbSet.Remove(chat);
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

        public async Task<bool> Contains(Guid id)
        {
            var chat = await DbSet.FirstOrDefaultAsync(x => x.Id == id);
            return chat != null;
        }
    }
}