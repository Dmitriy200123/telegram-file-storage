using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileStorageApp.Data.InfoStorage.Models
{
    public class File : IModel
    {
        public string Name { get; set; }
        public string Extension { get; set; }
        public string Type { get; set; }
        public DateTime UploadDate { get; set; }
        public Guid FileSenderId { get; set; }
        public Guid ChatId { get; set; }
        public Guid Id { get; set; }
        public virtual FileSender FileSender { get; set; }
        public virtual Chat Chat { get; set; }
    }
}