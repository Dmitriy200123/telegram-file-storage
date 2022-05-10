using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SearchDocumentsAPI.Services.DocumentsSearch;
using Swashbuckle.AspNetCore.Annotations;

namespace SearchDocumentsAPI.Controllers
{
    /// <summary>
    /// API поиска текстовых документов.
    /// </summary>
    [Route("api/search/documents")]
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
        /// Ищет документы по названию или содержимому.
        /// </summary>
        [HttpGet]
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
        /// Проверяет, содержится ли одна из строк в названии документа.
        /// </summary>
        [HttpGet("{documentId:guid}/containsInName")]
        [SwaggerResponse(StatusCodes.Status200OK, "Возвращает результат проверки", typeof(bool))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Произошла неизвестная ошибка")]
        public async Task<IActionResult> ContainsInDocumentName(Guid documentId, [FromQuery] params string[] queries)
        {
            var result = await _documentsSearchService.ContainsInDocumentNameByIdAsync(documentId, queries);

            return result.ResponseCode switch
            {
                HttpStatusCode.OK => Ok(result.Value),
                _ => throw new ArgumentException("Unknown response code")
            };
        }
    }
}