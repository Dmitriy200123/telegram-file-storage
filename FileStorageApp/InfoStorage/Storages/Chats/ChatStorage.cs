using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Config;
using FileStorageApp.Data.InfoStorage.Models;
using Microsoft.EntityFrameworkCore;

namespace FileStorageApp.Data.InfoStorage.Storages.Chats
{
    internal class ChatStorage : BaseStorage<Chat>, IChatStorage
    {
        internal ChatStorage(IDataBaseConfig dataBaseConfig) : base(dataBaseConfig)
        {
        }

        public new async Task<List<Chat>> GetAllAsync()
        {
            var list = await base.GetAllAsync();
            return list
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .ToList();
        }

        public Task<List<Chat>> GetByChatNameSubstringAsync(string subString)
        {
            if (subString == null)
                throw new ArgumentException("Substring can not be null");
            return DbSet
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .Where(x => x.Name.Contains(subString))
                .ToListAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Chat>(entity =>
            {
                entity.ToTable("Chats");
                entity.HasMany(x => x.Files)
                    .WithOne(x => x.Chat)
                    .HasForeignKey(x => x.ChatId);
                entity.Property(e => e.Name).HasMaxLength(255);
            });
        }
    }
}