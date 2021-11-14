using FileStorageApp.Data.InfoStorage.Enums;

namespace FileStorageAPI.Providers
{
    /// <inheritdoc />
    public class FileTypeProvider : IFileTypeProvider
    {
        /// <inheritdoc />
        public FileType GetFileType(string extension)
        {
            if (extension.Contains("image"))
                return FileType.Image;
            if (extension.Contains("video"))
                return FileType.Video;
            if (extension.Contains("audio"))
                return FileType.Audio;
            return FileType.Document;
        }
    }
}