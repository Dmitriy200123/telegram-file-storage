using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using File = FileStorageAPI.Models.File;

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
        /// <returns></returns>
        Task<RequestResult<List<File>>> GetFiles(FileSearchParameters fileSearchParameters);
        
        /// <summary>
        /// Возвращает файл по его идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор файла</param>
        /// <returns></returns>
        Task<RequestResult<File>> GetFileById(Guid id);
        
        /// <summary>
        /// Создает файл, сохраняет его в хранилище и возвращает информацию о нем
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<RequestResult<File>> CreateFile(IFormFile model);
        
        /// <summary>
        /// Обновляет имся файла и возвращает информацию о нем
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        Task<RequestResult<File>> UpdateFile(Guid id, string fileName);
        
        /// <summary>
        /// Удаляет чат и возвращает информацию об удаленном файле
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<RequestResult<File>> DeleteFile(Guid id);
    }
}