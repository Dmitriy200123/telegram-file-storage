using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FileStorageAPI;

namespace SearchDocumentsAPI.Services.DocumentsSearch
{
    /// <summary>
    /// Сервис для работы с поиском документов.
    /// </summary>
    public interface IDocumentsSearchService
    {
        /// <summary>
        /// Ищет совпадающие документы по запросу.
        /// </summary>
        /// <param name="query">Запрос поиска</param>
        /// <returns></returns>
        Task<RequestResult<IEnumerable<Guid>>> FindMatchingDocumentsAsync(string query);
        
        /// <summary>
        /// Проверяет, содержится ли один из текстов в названии документе по id
        /// </summary>
        /// <param name="documentId">Id документа</param>
        /// <param name="texts">Список текстов</param>
        /// <returns></returns>
        Task<RequestResult<bool>> ContainsInDocumentNameByIdAsync(Guid documentId, string[] texts);
    }
}