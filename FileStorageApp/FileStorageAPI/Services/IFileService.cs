using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        Task<RequestResult<List<FileInfo>>> GetFileInfosAsync(FileSearchParameters fileSearchParameters, int skip, int take);

        /// <summary>
        /// Возвращает информацию о файле по его идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор файла</param>
        Task<RequestResult<FileInfo>> GetFileInfoByIdAsync(Guid id);

        /// <summary>
        /// Возвращает ссылку для скачивания файла.
        /// </summary>
        /// <param name="id">Идентификатор файла</param>
        Task<RequestResult<string>> GetFileDownloadLinkByIdAsync(Guid id);

        /// <summary>
        /// Создает файл, сохраняет его в хранилище и возвращает информацию о нем.
        /// </summary>
        /// <param name="model">Файл</param>
        Task<RequestResult<(string Uri, FileInfo Info)>> CreateFileAsync(IFormFile model);

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
    }
}