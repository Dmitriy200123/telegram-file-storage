using FileStorageAPI.Models;
using FileStorageApp.Data.InfoStorage.Models;

namespace FileStorageAPI.Converters
{
    /// <summary>
    /// Конвертер модели DocumentClassification в ClassificationInfo
    /// </summary>
    public interface IClassificationToClassificationInfoConverter
    {
        /// <summary>
        /// Конвертирует модель DocumentClassification в ClassificationInfo
        /// </summary>
        /// <param name="classification">Модель классификации</param>
        /// <returns></returns>
        ClassificationInfo? ConvertToClassificationInfo(DocumentClassification? classification);
    }
}