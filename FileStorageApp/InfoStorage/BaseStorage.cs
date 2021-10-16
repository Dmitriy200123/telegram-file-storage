using FileStorageApp.Data.InfoStorage.Config;
using Microsoft.EntityFrameworkCore;

namespace FileStorageApp.Data.InfoStorage
{
    internal abstract class BaseStorage<T> : DbContext
        where T : class
    {
        protected DbSet<T> DbSet { get; set; }
        private readonly string _connectionLink;

        protected BaseStorage(IDataBaseConfig dataBaseConfig)
        {
            _connectionLink = dataBaseConfig.GetConnectionString();
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_connectionLink);
        }
    }
}