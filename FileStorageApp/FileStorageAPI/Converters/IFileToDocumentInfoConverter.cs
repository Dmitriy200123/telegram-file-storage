using FileStorageAPI.Models;
using FileStorageApp.Data.InfoStorage.Models;

namespace FileStorageAPI.Converters
{
    /// <summary>
    /// Конвертер модели File в DocumentInfo
    /// </summary>
    public interface IFileToDocumentInfoConverter
    {
        /// <summary>
        /// Конвертирует модель File в DocumentInfo
        /// </summary>
        /// <param name="file">Модель файла</param>
        /// <returns></returns>
        DocumentInfo ConvertToDocumentInfo(File file);
    }
}