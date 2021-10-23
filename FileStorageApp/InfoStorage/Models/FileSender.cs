using System;

namespace FileStorageApp.Data.InfoStorage.Models
{
    public class FileSender : IModel
    {
        public string TelegramUserName { get; set; }
        public string FullName { get; set; }
        public Guid Id { get; set; }
    }
}