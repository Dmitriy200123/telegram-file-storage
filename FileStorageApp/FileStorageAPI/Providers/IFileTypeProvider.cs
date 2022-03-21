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
        /// <param name="fileName">расширение файла</param>
        FileType GetFileType(string fileName);
    }
}