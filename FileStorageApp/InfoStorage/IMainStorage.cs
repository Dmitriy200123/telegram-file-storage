using FileStorageApp.Data.InfoStorage.Models;
using Microsoft.EntityFrameworkCore;

namespace FileStorageApp.Data.InfoStorage
{
    internal interface IMainStorage
    {
        public DbSet<File> Files { get; set; }
        public DbSet<FileSender> FileSenders { get; set; }
        public DbSet<Chat> Chats { get; set; }
    }
}