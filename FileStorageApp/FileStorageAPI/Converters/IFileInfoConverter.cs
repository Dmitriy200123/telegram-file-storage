using FileStorageAPI.Models;

namespace FileStorageAPI.Converters
{
    /// <summary>
    /// Конвертор для преобразования информации о файлах в API-контракты.
    /// </summary>
    public interface IFileInfoConverter
    {
        /// <summary>
        /// Конвертирует информацию о файле в API-контракты.
        /// </summary>
        /// <param name="file">Информация о файле</param>
        FileInfo ConvertFileInfo(FileStorageApp.Data.InfoStorage.Models.File file);
    }
}