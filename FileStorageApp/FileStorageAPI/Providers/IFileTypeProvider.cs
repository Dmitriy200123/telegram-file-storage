using FileStorageApp.Data.InfoStorage.Enums;

namespace FileStorageAPI.Providers
{
    /// <summary>
    /// Поставщик типа файла
    /// </summary>
    public interface IFileTypeProvider
    {
        /// <summary>
        /// Возвращает тип файла по его расширению
        /// </summary>
        /// <param name="extension">расширение файла</param>
        /// <returns></returns>
        FileType GetFileType(string extension);
    }
}