using System;
using System.Threading.Tasks;
using DocumentsIndex.Model;

namespace SearchDocumentsAPI.Services
{
    /// <summary>
    /// Сервис для взаимодействия с индексацией документов.
    /// </summary>
    public interface IDocumentsIndexService
    {
        /// <summary>
        /// Индексирует документ для дальнейшего использования при поиске.
        /// </summary>
        /// <param name="document">Документ, который нужно проиндексировать</param>
        Task<bool> IndexDocumentAsync(Document document);

        /// <summary>
        /// Удаляет документ из индексации.
        /// </summary>
        /// <param name="id">Id документа</param>
        Task<bool> DeleteAsync(Guid id);
    }
}