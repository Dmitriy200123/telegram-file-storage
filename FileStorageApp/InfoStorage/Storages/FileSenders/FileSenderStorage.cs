using System;
using System.Linq;
using FileStorageApp.Data.InfoStorage.Config;
using FileStorageApp.Data.InfoStorage.Models;
using Microsoft.EntityFrameworkCore;

namespace FileStorageApp.Data.InfoStorage.Storages.FileSenders
{
    internal class FileSenderStorage : BaseStorage<FileSender>, IFileSenderStorage
    {
        public FileSender GetFileSenderById(Guid id)
        {
            var fileSender = DbSet.FirstOrDefault(x => x.Id == id);
            if (fileSender == null)
                throw new ArgumentException($"User with id {id} not found");
            return fileSender;
        }

        internal FileSenderStorage(IDataBaseConfig dataBaseConfig) : base(dataBaseConfig)
        {
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