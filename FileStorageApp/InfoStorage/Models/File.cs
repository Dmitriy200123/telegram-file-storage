using System;

namespace FileStorageApp.Data.InfoStorage.Models
{
    public class File
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public string Type { get; set; }
        public DateTime UploadDate { get; set; }
        public Guid SenderId { get; set; }
        public Guid ChatId { get; set; }
    }
}