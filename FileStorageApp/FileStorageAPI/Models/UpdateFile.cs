using System;

namespace FileStorageAPI.Models
{
    public class UpdateFile
    {
        public Guid FileId { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string SenderName { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}