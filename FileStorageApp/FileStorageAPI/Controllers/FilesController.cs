using System;
using System.Net;
using System.Threading.Tasks;
using FileStorageAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileStorageAPI.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("files")]
    public class FilesController : Controller
    {
        private readonly IFileService _fileService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileService"></param>
        public FilesController(IFileService fileService)
        {
            _fileService = fileService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        [HttpGet]
        public async Task<IActionResult> GetFiles()
        {
            var files = await _fileService.GetFiles();
            return files.ResponseCode switch
            {
                HttpStatusCode.OK => Ok(files.Value),
                _ => throw new ArgumentException("Unknown response code")
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetFileById(Guid id)
        {
            var file = await _fileService.GetFileById(id);
            return file.ResponseCode switch
            {
                HttpStatusCode.OK => Ok(file.Value),
                HttpStatusCode.NotFound => NotFound(file.Message),
                _ => throw new ArgumentException("Unknown response code")
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        [HttpPost]
        public async Task<IActionResult> PostFile(IFormFile file)
        {
            var uploadedFile = await _fileService.CreateFile(file);
            return uploadedFile.ResponseCode switch
            {
                HttpStatusCode.Created => Created(uploadedFile.Value!.DownloadLink, uploadedFile.Value),
                _ => throw new ArgumentException("Unknown response code")
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> PutFile(Guid id, [FromBody]string fileName)
        {
            var file = await _fileService.UpdateFile(id, fileName);
            return file.ResponseCode switch
            {
                HttpStatusCode.Created => Created(file.Value!.DownloadLink, file.Value),
                HttpStatusCode.NotFound => NotFound(file.Message),
                _ => throw new ArgumentException("Unknown response code")
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteFile(Guid id)
        {
            var file = await _fileService.DeleteFile(id);
            return file.ResponseCode switch
            {
                HttpStatusCode.NoContent => NoContent(),
                _ => throw new ArgumentException("Unknown response code")
            };
        }
    }
}