using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using File = FileStorageAPI.Models.File;

namespace FileStorageAPI.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<RequestResult<List<File>>> GetFiles();
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<RequestResult<File>> GetFileById(Guid id);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<RequestResult<File>> CreateFile(IFormFile model);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        Task<RequestResult<File>> UpdateFile(Guid id, string fileName);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<RequestResult<File>> DeleteFile(Guid id);
    }
}