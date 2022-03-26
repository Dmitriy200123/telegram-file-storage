using System.Collections.Generic;
using FileStorageApp.Data.InfoStorage.Enums;
using Microsoft.Extensions.Configuration;

namespace FileStorageAPI.Providers
{
    /// <inheritdoc />
    public class FileTypeProvider : IFileTypeProvider
    {
        /// <inheritdoc />
        public FileType GetFileType(string fileName)
        {
            var mimeType = MimeTypes.GetMimeType(fileName);
            var fileTypes = Settings.Configuration.GetSection("SupportedFileTypes").Get<List<string>>();
            
            if (fileTypes.Contains(mimeType))
                return FileType.TextDocument;
            
            if (mimeType.Contains("image"))
                return FileType.Image;
            if (mimeType.Contains("video"))
                return FileType.Video;
            if (mimeType.Contains("audio"))
                return FileType.Audio;
            return FileType.Document;
        }
    }
}