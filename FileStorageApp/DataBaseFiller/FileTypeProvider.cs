using FileStorageApp.Data.InfoStorage.Enums;

namespace DataBaseFiller
{
    public static class FileTypeProvider
    {
        public static FileType GetFileType(string extension)
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