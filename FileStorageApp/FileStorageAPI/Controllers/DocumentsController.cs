using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using FileStorageAPI.Models;
using FileStorageAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FileStorageAPI.Controllers
{
    /// <summary>
    /// API информации о файлах загруженных во внешний анализатор
    /// </summary>
    [ApiController]
    [Route("api/documents")]
    [SwaggerTag("Информация о документах")]
    [Authorize]
    public class DocumentsController : Controller
    {
        private readonly IDocumentsService _documentsService;

        /// <summary>
        /// Конструктор контроллера
        /// </summary>
        /// <param name="documentsService">Сервис для взаимодействия с файлами</param>
        public DocumentsController(IDocumentsService documentsService)
        {
            _documentsService = documentsService ?? throw new ArgumentNullException(nameof(documentsService));
        }
        
        /// <summary>
        /// Возвращает количество файлов по заданным параметрам
        /// </summary>
        /// <exception cref="ArgumentException">Может выброситься, если контроллер не ожидает такой HTTP код</exception>
        [HttpGet("count")]
        [SwaggerResponse(StatusCodes.Status200OK, "Возвращает количество документов, содержащихся в хранилище", typeof(int))]
        public async Task<IActionResult> GetFilesCount([FromQuery] DocumentSearchParameters fileSearchParameters)
        {
            var count = await _documentsService.GetFilesCountAsync(fileSearchParameters, Request);

            return count.ResponseCode switch
            {
                HttpStatusCode.OK => Ok(count.Value),
                _ => throw new ArgumentException("Unknown response code")
            };
        }
        /// <summary>
        /// Возвращает список файлов, к которым пользователь имеет доступ.
        /// </summary>
        /// <param name="fileSearchParameters">Параметры поиска файлов</param>
        /// <param name="skip">Количество пропускаемых элементов</param>
        /// <param name="take">Количество возвращаемых элементов</param>
        /// <exception cref="ArgumentException">Может выброситься, если контроллер не ожидает такой HTTP код</exception>
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Возвращает все доступные файлы для текущего пользователя", typeof(FileInfo))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Если skip или take меньше 0", typeof(string))]
        public async Task<IActionResult> GetFileInfos([FromQuery] DocumentSearchParameters fileSearchParameters, [FromQuery, Required] int skip, [FromQuery, Required] int take)
        {
            var files = await _documentsService.GetFileInfosAsync(fileSearchParameters, skip, take, Request);

            return files.ResponseCode switch
            {
                HttpStatusCode.OK => Ok(files.Value),
                HttpStatusCode.BadRequest => BadRequest(files.Message),
                _ => throw new ArgumentException("Unknown response code")
            };
        }
    }
}