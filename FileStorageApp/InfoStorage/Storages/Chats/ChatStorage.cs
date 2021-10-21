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
            return DbSet.ToListAsync();
        }

        public Task<Chat> GetById(Guid id)
        {
            return DbSet.FirstAsync(x => x.Id == id);
        }

        public Task<List<Chat>> GetBySubString(string subString)
        {
            return DbSet.Where(x => x.Name.Contains(subString)).ToListAsync();
        }

        public ValueTask<EntityEntry<Chat>> Add(Chat chat)
        {
            return DbSet.AddAsync(chat);
        }

        public async Task<EntityEntry<Chat>> Update(Guid id, string? name = null, Guid? imageId = null)
        {
            var chat = await DbSet.FirstOrDefaultAsync(x => x.Id == id);
            if (name != null)
                chat.Name = name;
            if (imageId != null)
                chat.ImageId = imageId.Value;
            return DbSet.Update(chat);
        }

        public async Task<EntityEntry<Chat>> Delete(Guid id)
        {
            var chat = await DbSet.FirstAsync(x => x.Id == id);
            return DbSet.Remove(chat);
        }

        public async Task<bool> Contains(Guid id)
        {
            var chat = await DbSet.FirstOrDefaultAsync(x => x.Id == id);
            return chat != null;
        }
    }
}