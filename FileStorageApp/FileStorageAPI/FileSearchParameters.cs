using System;
using FileStorageApp.Data.InfoStorage.Enums;

namespace FileStorageAPI
{
    public class FileSearchParameters
    {
        public string? FileName { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public Guid[]? SenderIds { get; set; }
        public Guid[]? ChatIds { get; set; }
        public FileType[]? Categories { get; set; }
    }
}