using System;
using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Config;
using FileStorageApp.Data.InfoStorage.Models;
using Microsoft.EntityFrameworkCore;

namespace FileStorageApp.Data.InfoStorage.Storages
{
    internal abstract class BaseStorage<T> : DbContext, IInfoStorage<T> where T : class, IModel
    {
        private readonly IDataBaseConfig _dataBaseConfig;

        protected BaseStorage(IDataBaseConfig dataBaseConfig)
        {
            _dataBaseConfig = dataBaseConfig;
        }

        protected DbSet<T> DbSet { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Chat>().HasAlternateKey(chat => chat.TelegramId);
            modelBuilder.Entity<FileSender>().HasAlternateKey(sender => sender.TelegramId);
            modelBuilder
                .Entity<FileSender>()
                .HasMany(sender => sender.Chats)
                .WithMany(chat => chat.Senders)
                .UsingEntity(builder => builder.ToTable("SenderAndChat"));

            modelBuilder
                .Entity<DocumentClassification>()
                .HasAlternateKey(classification => classification.Name);

            modelBuilder
                .Entity<DocumentClassificationWord>()
                .HasAlternateKey(word => new { word.ClassificationId, word.Value });
        }

        public async Task<bool> AddAsync(T entity, bool writeException = true)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            await DbSet.AddAsync(entity);
            return await TrySaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            DbSet.Update(entity);
            return await TrySaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity is null)
                throw new ArgumentException($"There is no entity with ID {id} in the database");

            DbSet.Remove(entity);
            return await TrySaveChangesAsync();
        }

        protected async Task<bool> TrySaveChangesAsync()
        {
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

        public virtual async Task<bool> ContainsAsync(Guid id)
        {
            var entity = await DbSet.FindAsync(id);
            return entity is not null;
        }

        protected virtual async Task<T> GetByIdAsync(Guid id)
        {
            return await DbSet.FindAsync(id);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_dataBaseConfig.ConnectionString);
        }
    }
}