using FileStorageAPI.Models;

namespace FileStorageAPI.Converters
{
    /// <summary>
    /// Класс для конвертации модели поиска документа в модель поиска файла
    /// </summary>
    public interface IDocumentToFileConverter
    {
        /// <summary>
        /// метод конфертации модели поиска документа в модель поиска файл
        /// </summary>
        /// <param name="parameters">модель поиска документа</param>
        /// <returns></returns>
        FileSearchParameters ToFileSearchParameters(DocumentSearchParameters parameters);

        /// <summary>
        /// Преобразовывает модель файла в модель документа
        /// </summary>
        /// <param name="fileInfo">модель файла</param>
        /// <returns></returns>
        DocumentInfo ToDocumentModel(FileInfo fileInfo);
    }
}