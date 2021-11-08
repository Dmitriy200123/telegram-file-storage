using System;
using FileStorageApp.Data.InfoStorage.Enums;

namespace FileStorageAPI.Models
{
    public class File
    {
        public Guid FileId { get; set; }
        public string FileName { get; set; }
        public FileType FileType { get; set; }
        public Sender Sender { get; set; }
        public DateTime UploadDate { get; set; }
        public Chat Chat { get; set; }
        public string DownloadLink { get; set; }
    }
}