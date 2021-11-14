using FileStorageAPI.Models;

namespace FileStorageAPI.Converters
{
    /// <summary>
    /// 
    /// </summary>
    public interface IFileConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        File ConvertFile(FileStorageApp.Data.InfoStorage.Models.File file);
    }
}