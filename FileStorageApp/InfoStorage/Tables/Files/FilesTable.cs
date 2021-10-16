using System;
using System.Linq;
using FileStorageApp.Data.InfoStorage.Config;
using FileStorageApp.Data.InfoStorage.Models;
using Microsoft.EntityFrameworkCore;

namespace FileStorageApp.Data.InfoStorage.Tables.Files
{
    internal class FilesTable : BaseStorage<File>, IFilesTable
    {
        public File GetFileById(Guid id)
        {
            var fileSender = DbSet.FirstOrDefault(x => x.Id == id);
            if (fileSender == null)
                throw new ArgumentException($"File with id {id} not found");
            return fileSender;
        }

        internal FilesTable(IDataBaseConfig dataBaseConfig) : base(dataBaseConfig)
        {
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