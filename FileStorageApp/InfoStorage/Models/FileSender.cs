using System;
using System.Collections.Generic;

namespace FileStorageApp.Data.InfoStorage.Models
{
    public class FileSender : IModel
    {
        public string TelegramUserName { get; set; }
        public string FullName { get; set; }
        public Guid Id { get; set; }
        public List<File> Files { get; set; }
    }
}