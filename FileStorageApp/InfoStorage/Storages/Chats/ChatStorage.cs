using System;
using System.Linq;
using FileStorageApp.Data.InfoStorage.Config;
using FileStorageApp.Data.InfoStorage.Models;
using Microsoft.EntityFrameworkCore;

namespace FileStorageApp.Data.InfoStorage.Storages.Chats
{
    internal class ChatStorage : BaseStorage<Chat>, IChatStorage
    {
        public Chat GetChatById(Guid id)
        {
            var fileSender = DbSet.FirstOrDefault(x => x.Id == id);
            if (fileSender == null)
                throw new ArgumentException($"Chat with id {id} not found");
            return fileSender;
        }

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
    }
}