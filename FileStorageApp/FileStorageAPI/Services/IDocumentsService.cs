using System.Collections.Generic;
using System.Threading.Tasks;
using API;
using FileStorageAPI.Models;
using Microsoft.AspNetCore.Http;

namespace FileStorageAPI.Services
{
    /// <summary>
    /// Сервис для взаимодействия с файлами загруженных во внешний анализатор
    /// </summary>
    public interface IDocumentsService
    {
        /// <summary>
        /// Возвращает кол-вой файлов по заданным параметрам
        /// </summary>
        /// <param name="documentSearchParameters">Параметры для поиска</param>
        /// <param name="request">Запрос</param>
        /// <returns></returns>
        Task<RequestResult<int>> GetFilesCountAsync(DocumentSearchParameters documentSearchParameters, HttpRequest request);
        
        /// <summary>
        /// Возвращает весь список документов.
        /// </summary>
        Task<RequestResult<List<FileInfo>>> GetFileInfosAsync(DocumentSearchParameters documentSearchParameters, int skip, int take, HttpRequest request);
    }
}