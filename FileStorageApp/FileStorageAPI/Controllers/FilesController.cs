using System;
using System.Net;
using System.Threading.Tasks;
using FileStorageAPI.Models;
using FileStorageAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FileStorageAPI.Controllers
{
    /// <summary>
    /// API информации о файлах из Telegram.
    /// </summary>
    [Route("files")]
    [SwaggerTag("Информация об файлах из Telegram")]
    public class FilesController : Controller
    {
        private readonly IFileService _fileService;

        /// <summary>
        /// Инициализирует новый экземляр класса <see cref="FilesController"/>.
        /// </summary>
        /// <param name="fileService">Сервис для взаимодействия с информацией о файлах</param>
        public FilesController(IFileService fileService)
        {
            _fileService = fileService;
        }

        /// <summary>
        /// Возвращает список файлов, к которым пользователь имеет доступ
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Может выброситься, если контроллер не ожидает такой HTTP код</exception>
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Возвращает все доступные файлы для текущего пользователя", typeof(File))]
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
        /// Возвращает файл по id.
        /// </summary>
        /// <param name="id">Идентификатор файла</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Может выброситься, если контроллер не ожидает такой HTTP код</exception>
        [HttpGet("{id:guid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Возвращает файл по заданному идентификатору", typeof(File))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Если файл с таким идентификатором не найден", typeof(string))]
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
        /// Добавление файла в базу данныых и физическое хранилище
        /// </summary>
        /// <param name="file">Файл</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Может выброситься, если контроллер не ожидает такой HTTP код</exception>
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status201Created, "Возвращает информацию о созданном файле", typeof(File))]
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
        /// Обновление название файла
        /// </summary>
        /// <param name="id">Идентификатор файла</param>
        /// <param name="fileName">Новое имя файла</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Может выброситься, если контроллер не ожидает такой HTTP код</exception>
        [HttpPut("{id:guid}")]
        [SwaggerResponse(StatusCodes.Status201Created, "Возвращает информацию об обновленном файле", typeof(File))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Если файл с таким идентификатором не найден", typeof(string))]
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
        /// Удаление файла
        /// </summary>
        /// <param name="id">Идентификатор файла</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Может выброситься, если контроллер не ожидает такой HTTP код</exception>
        [HttpDelete("{id:guid}")]
        [SwaggerResponse(StatusCodes.Status201Created, "Возвращает информацию об удаленном файле", typeof(File))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Если файл с таким идентификатором не найден", typeof(string))]
        public async Task<IActionResult> DeleteFile(Guid id)
        {
            var file = await _fileService.DeleteFile(id);
            return file.ResponseCode switch
            {
                HttpStatusCode.NoContent => NoContent(),
                HttpStatusCode.NotFound => NotFound(file.Message),
                _ => throw new ArgumentException("Unknown response code")
            };
        }
    }
}