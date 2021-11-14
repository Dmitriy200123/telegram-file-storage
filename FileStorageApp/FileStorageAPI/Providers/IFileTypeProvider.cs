using FileStorageApp.Data.InfoStorage.Enums;

namespace FileStorageAPI.Providers
{
    /// <summary>
    /// 
    /// </summary>
    public interface IFileTypeProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        FileType GetFileType(string extension);
    }
}