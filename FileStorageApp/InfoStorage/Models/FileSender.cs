using System;

namespace FileStorageApp.Data.InfoStorage.Models
{
    public class FileSender
    {
        public Guid Id { get; set; }
        public string TelegramUserName { get; set; }
        public string FullName { get; set; }
    }
}