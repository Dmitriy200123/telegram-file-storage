using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API;
using FileStorageAPI.Models;
using Microsoft.AspNetCore.Http;

namespace FileStorageAPI.Services
{
    /// <summary>
    /// Сервис для взаимодействия с файлами типа "Текстовый документ".
    /// </summary>
    public interface IDocumentsService
    {
        /// <summary>
        /// Возвращает количество файлов типа "Текстовый документ" по заданным параметрам
        /// </summary>
        /// <param name="documentSearchParameters">Параметры для поиска</param>
        /// <param name="request">Запрос</param>
        /// <returns></returns>
        Task<RequestResult<int>> GetDocumentsCountAsync(DocumentSearchParameters documentSearchParameters, HttpRequest request);

        /// <summary>
        /// Возвращает файлы типа "Текстовый документ".
        /// </summary>
        Task<RequestResult<List<DocumentInfo>>> GetDocumentInfosAsync(DocumentSearchParameters documentSearchParameters, int skip, int take, HttpRequest request);

        /// <summary>
        /// Возвращает документ по Id
        /// </summary>
        /// <param name="id">Id документа</param>
        Task<RequestResult<DocumentInfo>> FindDocumentById(Guid id);

        /// <summary>
        /// Возвращает классификацию по Id документа
        /// </summary>
        /// <param name="documentId">Id документа</param>
        Task<RequestResult<ClassificationInfo?>> FindClassificationByDocumentId(Guid documentId);
    }
}