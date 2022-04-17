using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API;
using FileStorageAPI.Models;
using Microsoft.AspNetCore.Http;

namespace FileStorageAPI.Services
{
    /// <summary>
    /// Сервис для взаимодействия с информацией о файлах.
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// Возвращает весь список файлов.
        /// </summary>
        Task<RequestResult<List<FileInfo>>> GetFileInfosAsync(FileSearchParameters fileSearchParameters, int skip,
            int take, HttpRequest request);

        /// <summary>
        /// Возвращает информацию о файле по его идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор файла</param>
        /// <param name="request"></param>
        Task<RequestResult<FileInfo>> GetFileInfoByIdAsync(Guid id, HttpRequest request);

        /// <summary>
        /// Возвращает ссылку для скачивания файла.
        /// </summary>
        /// <param name="id">Идентификатор файла</param>
        /// <param name="request"></param>
        Task<RequestResult<string>> GetFileDownloadLinkByIdAsync(Guid id, HttpRequest request);

        /// <summary>
        /// Создает файл, сохраняет его в хранилище и возвращает информацию о нем.
        /// </summary>
        /// <param name="model">Файл</param>
        /// <param name="request"></param>
        Task<RequestResult<(string Uri, FileInfo Info)>> CreateFileAsync(IFormFile model, HttpRequest request);

        /// <summary>
        /// Обновляет имя файла и возвращает информацию о нем.
        /// </summary>
        /// <param name="id">Идентификатор файла</param>
        /// <param name="fileName">Имя файла</param>
        Task<RequestResult<(string Uri, FileInfo Info)>> UpdateFileAsync(Guid id, string fileName);

        /// <summary>
        /// Удаляет чат и возвращает информацию об удаленном файле.
        /// </summary>
        /// <param name="id">Идентификатор файла</param>
        Task<RequestResult<FileInfo>> DeleteFileAsync(Guid id);

        /// <summary>
        /// Возвращает количество файлов, содержащихся в хранилище.
        /// </summary>
        Task<RequestResult<int>> GetFilesCountAsync(FileSearchParameters fileSearchParameters, HttpRequest request);

        /// <summary>
        /// Возвращает список названий файлов.
        /// </summary>
        Task<RequestResult<List<string>>> GetFileNamesAsync(HttpRequest request);

        /// <summary>
        /// Возвращает список типов файлов.
        /// </summary>
        RequestResult<FileTypeDescription[]> GetFilesTypes();

        /// <summary>
        /// Возвращает сохраненную ссылку
        /// </summary>
        /// <param name="id">Id ссылки</param>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<RequestResult<string>> GetLink(Guid id, HttpRequest request);

        /// <summary>
        /// Возвращает сохраненное сообщение
        /// </summary>
        /// <param name="id">Id сообщения</param>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<RequestResult<string>> GetMessage(Guid id, HttpRequest request);

        /// <summary>
        /// Создает новое сообщение и возвращает ссылку на него и id
        /// </summary>
        /// <param name="uploadTextData">Название сообщения и его текст</param>
        /// <param name="request">Пришедший запрос</param>
        /// <returns></returns>
        Task<RequestResult<(string Uri, Guid Guid)>> PostMessage(UploadTextData uploadTextData, HttpRequest request);
        
        /// <summary>
        /// Создает новую ссылку и возвращает ссылку на нее и id
        /// </summary>
        /// <param name="uploadTextData">Название ссылки и ее текст</param>
        /// <param name="request">Пришедший запрос</param>
        /// <returns></returns>
        Task<RequestResult<(string Uri, Guid Guid)>> PostLink(UploadTextData uploadTextData, HttpRequest request);

        
        /// <summary>
        /// Метод который по фильтрам возвращает количество файлов являющихся документами
        /// </summary>
        /// <param name="fileSearchParameters">параметры для поиска файлов</param>
        /// <param name="guidsToFind">идентификаторы документов среди которых нужно произвоить поиск</param>
        /// <param name="request">запрос</param>
        /// <returns></returns>
        Task<RequestResult<int>> GetDocumentsCountByParametersAndIds(FileSearchParameters fileSearchParameters,
            List<Guid> guidsToFind, HttpRequest request);

        /// <summary>
        /// Метод который по фильтрам возвращает информацию о файлах являющихся документами
        /// </summary>
        /// <param name="fileSearchParameters">параметры для поиска файлов</param>
        /// <param name="fileIds">идентификаторы документов среди которых нужно произвоить поиск</param>
        /// <param name="request">запрос</param>
        /// <param name="skip">сколько документов пропустить</param>
        /// <param name="take">сколько документов взять</param>
        /// <returns></returns>
        Task<RequestResult<List<FileInfo>>> GetDocumentsByParametersAndIds(
            FileSearchParameters fileSearchParameters, List<Guid> fileIds, HttpRequest request, int skip, int take);
    }
}