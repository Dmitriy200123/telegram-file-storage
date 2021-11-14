using System;
using FileStorageApp.Data.InfoStorage.Enums;

namespace FileStorageAPI.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class File
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid FileId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public FileType FileType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Sender Sender { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime UploadDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Chat Chat { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DownloadLink { get; set; }
    }
}