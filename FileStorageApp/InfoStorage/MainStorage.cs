using FileStorageApp.Data.InfoStorage.Config;
using FileStorageApp.Data.InfoStorage.Models;
using Microsoft.EntityFrameworkCore;

namespace FileStorageApp.Data.InfoStorage
{
    internal class MainStorage : DbContext, IMainStorage
    {
        public DbSet<File> Files { get; set; }
        public DbSet<FileSender> FileSenders { get; set; }
        public DbSet<Chat> Chats { get; set; }
        private readonly string _connectionLink;
        public MainStorage(IDataBaseConfig dataBaseConfig)
        {
            Database.EnsureCreated();
            _connectionLink = dataBaseConfig.GetConnectionString();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_connectionLink);
        }
    }
}