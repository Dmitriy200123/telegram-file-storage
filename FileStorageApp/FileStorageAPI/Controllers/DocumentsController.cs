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
    [Route("api/files/documents")]
    [SwaggerTag("Информация о документах")]
    [Authorize]
    public class DocumentsController : Controller
    {
        private readonly IDocumentsService _documentsService;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="DocumentsController"/>.
        /// </summary>
        /// <param name="documentsService">Сервис для взаимодействия с документами</param>
        public DocumentsController(IDocumentsService documentsService)
        {
            _documentsService = documentsService ?? throw new ArgumentNullException(nameof(documentsService));
        }
        
        /// <summary>
        /// Возвращает количество документов по заданным параметрам
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
        /// Возвращает список документов, к которым пользователь имеет доступ.
        /// </summary>
        /// <param name="documentSearchParameters">Параметры поиска документов</param>
        /// <param name="skip">Количество пропускаемых элементов</param>
        /// <param name="take">Количество возвращаемых элементов</param>
        /// <exception cref="ArgumentException">Может выброситься, если контроллер не ожидает такой HTTP код</exception>
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Возвращает все доступные документы для текущего пользователя", typeof(FileInfo))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Если skip или take меньше 0", typeof(string))]
        public async Task<IActionResult> GetFileInfos([FromQuery] DocumentSearchParameters documentSearchParameters, [FromQuery, Required] int skip, [FromQuery, Required] int take)
        {
            var files = await _documentsService.GetFileInfosAsync(documentSearchParameters, skip, take, Request);

            return files.ResponseCode switch
            {
                HttpStatusCode.OK => Ok(files.Value),
                HttpStatusCode.BadRequest => BadRequest(files.Message),
                _ => throw new ArgumentException("Unknown response code")
            };
        }
    }
}