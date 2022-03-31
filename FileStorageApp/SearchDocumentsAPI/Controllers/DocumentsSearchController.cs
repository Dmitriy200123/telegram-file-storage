using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SearchDocumentsAPI.Services.DocumentsSearch;
using Swashbuckle.AspNetCore.Annotations;

namespace SearchDocumentsAPI.Controllers
{
    /// <summary>
    /// Управление поиском документов в ElasticSearch
    /// </summary>
    [Route("api/documents")]
    [SwaggerTag("Поиск документов")]
    [ApiController]
    public class DocumentsSearchController : ControllerBase
    {
        private readonly IDocumentsSearchService _documentsSearchService;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="DocumentsSearchController"/>
        /// </summary>
        /// <param name="documentsSearchService">Сервис для взаимодействия с хранилищем документов</param>
        public DocumentsSearchController(IDocumentsSearchService documentsSearchService)
        {
            _documentsSearchService = documentsSearchService ??
                                      throw new ArgumentNullException(nameof(documentsSearchService));
        }

        /// <summary>
        /// Ищет документы по названию и содержимому.
        /// </summary>
        [HttpGet("search")]
        [SwaggerResponse(StatusCodes.Status200OK, "Возвращает список id совпадающий документов", typeof(IEnumerable<Guid>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Произошла неизвестная ошибка")]
        public async Task<IActionResult> FindMatchingDocuments([FromQuery] string query)
        {
            var result = await _documentsSearchService.FindMatchingDocumentsAsync(query);

            return result.ResponseCode switch
            {
                HttpStatusCode.OK => Ok(result.Value),
                _ => throw new ArgumentException("Unknown response code")
            };
        }

        /// <summary>
        /// Проверяет, содержится ли один из текстов в названии документа.
        /// </summary>
        [HttpPost("{documentId:guid}/containsInName")]
        [SwaggerResponse(StatusCodes.Status200OK, "Возвращает результат проверки", typeof(bool))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Произошла неизвестная ошибка")]
        public async Task<IActionResult> ContainsInDocumentName(Guid documentId, [FromBody] string[] texts)
        {
            var result = await _documentsSearchService.ContainsInDocumentNameByIdAsync(documentId, texts);

            return result.ResponseCode switch
            {
                HttpStatusCode.OK => Ok(result.Value),
                _ => throw new ArgumentException("Unknown response code")
            };
        }
    }
}