using System;
using System.Threading.Tasks;
using DocumentsIndex.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SearchDocumentsAPI.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace SearchDocumentsAPI.Controllers
{
    /// <summary>
    /// Управление индексакцией документов в ElasticSearch
    /// </summary>
    [Route("api/documentIndex")]
    [SwaggerTag("Индексирование документов")]
    [ApiController]
    public class DocumentsIndexController : ControllerBase
    {
        private readonly IDocumentsIndexService _documentsIndexService;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="DocumentsIndexController"/>
        /// </summary>
        /// <param name="documentsIndexService">Сервис для работы взаимодействия с хранилищем индексации</param>
        public DocumentsIndexController(IDocumentsIndexService documentsIndexService)
        {
            _documentsIndexService = documentsIndexService ??
                                     throw new ArgumentNullException(nameof(documentsIndexService));
        }

        /// <summary>
        /// Индексирует документ.
        /// </summary>
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, "Документ проиндексировался")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Если документ не удалось проиндексировать")]
        [DisableRequestSizeLimit]
        [RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue, ValueLengthLimit = int.MaxValue)]
        public async Task<IActionResult> IndexDocument([FromBody] Document document)
        {
            var success = await _documentsIndexService.IndexDocumentAsync(document);

            return success switch
            {
                true => Ok(),
                false => BadRequest()
            };
        }

        /// <summary>
        /// Удаляет документ из индексирования.
        /// </summary>
        [HttpDelete("{id:guid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Документ удален из индексации")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Если документ не удален")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _documentsIndexService.DeleteAsync(id);

            return success switch
            {
                true => Ok(),
                false => BadRequest()
            };
        }
    }
}