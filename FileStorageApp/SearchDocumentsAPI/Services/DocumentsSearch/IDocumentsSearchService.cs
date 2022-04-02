using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API;

namespace SearchDocumentsAPI.Services.DocumentsSearch
{
    /// <summary>
    /// Сервис для работы с поиском текстовым документов.
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
        /// Проверяет, содержится ли одна из строк в названии документа по id.
        /// </summary>
        /// <param name="documentId">Id документа</param>
        /// <param name="queries">Список строк</param>
        /// <returns></returns>
        Task<RequestResult<bool>> ContainsInDocumentNameByIdAsync(Guid documentId, string[] queries);
    }
}