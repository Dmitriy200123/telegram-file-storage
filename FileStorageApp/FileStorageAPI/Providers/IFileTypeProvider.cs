using FileStorageApp.Data.InfoStorage.Enums;

namespace FileStorageAPI.Providers
{
    /// <summary>
    /// Поставщик типа файла
    /// </summary>
    public interface IFileTypeProvider
    {
        /// <summary>
        /// Возвращает тип файла по его имени
        /// </summary>
        /// <param name="fileName">Полное имя файла</param>
        FileType GetFileType(string fileName);
    }
}