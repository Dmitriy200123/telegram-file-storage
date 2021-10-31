using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileStorageApp.Data.InfoStorage.Models
{
    public class FileSender : IModel
    {
        public string TelegramUserName { get; set; }
        public string FullName { get; set; }
        public Guid Id { get; set; }
        public virtual ICollection<File> Files { get; set; }
    }
}