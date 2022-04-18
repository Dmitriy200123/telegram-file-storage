using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using DocumentClassificationsAPI.Models;
using DocumentClassificationsAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DocumentClassificationsAPI.Controllers
{
    /// <summary>
    /// API классификации документов
    /// </summary>
    [Route("api/documentClassifications")]
    [SwaggerTag("Классификация документов")]
    [ApiController]
    public class DocumentClassificationsController : ControllerBase
    {
        private readonly IDocumentClassificationsService _classificationsService;

        /// <summary>
        /// Инициализирует экземпляр класса <see cref="DocumentClassificationsController"/>
        /// </summary>
        /// <param name="classificationsService">Сервис для работы с хранилищем классификаций</param>
        public DocumentClassificationsController(IDocumentClassificationsService classificationsService)
        {
            _classificationsService = classificationsService;
        }

        /// <summary>
        /// Получение классификации по Id
        /// </summary>
        /// <param name="classificationId">Id классификации</param>
        /// <param name="includeClassificationWords">Включить в классификацию принадлежащий список слов</param>
        /// <returns></returns>
        [HttpGet("{classificationId:guid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Возвращает классификацию", typeof(Classification))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Возвращается, когда не удалось найти классификацию", typeof(string))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Произошла неизвестная ошибка")]
        public async Task<IActionResult> FindClassificationById(
            Guid classificationId,
            [FromQuery] bool includeClassificationWords = false
        )
        {
            var result = await _classificationsService
                .FindClassificationByIdAsync(classificationId, includeClassificationWords);

            return result.ResponseCode switch
            {
                HttpStatusCode.OK => Ok(result.Value),
                HttpStatusCode.BadRequest => BadRequest(result.Message),
                _ => throw new ArgumentException("Unknown response code")
            };
        }

        /// <summary>
        /// Поиск классификаций по строке
        /// </summary>
        /// <param name="query">Строка</param>
        /// <param name="skip">Количество пропускаемых элементов</param>
        /// <param name="take">Количество возвращаемых элементов</param>
        /// <param name="includeClassificationWords">Включить в классификации принадлежащие списки слов</param>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Возвращает cписок классификаций", typeof(IEnumerable<Classification>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Произошла неизвестная ошибка")]
        public async Task<IActionResult> FindClassificationsByQuery(
            [FromQuery] string query,
            [FromQuery, Required] int skip,
            [FromQuery, Required] int take,
            [FromQuery] bool includeClassificationWords = false
        )
        {
            var result = await _classificationsService
                .FindClassificationByQueryAsync(query, skip, take, includeClassificationWords);

            return result.ResponseCode switch
            {
                HttpStatusCode.OK => Ok(result.Value),
                _ => throw new ArgumentException("Unknown response code")
            };
        }

        /// <summary>
        /// Добавление классификации
        /// </summary>
        /// <param name="classification">Классификация</param>
        /// <returns></returns>
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, "Добавление прошло успешно")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Возвращается, если классификация с таким именем уже существует", typeof(string))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Произошла неизвестная ошибка")]
        public async Task<IActionResult> AddClassifications([FromBody] ClassificationInsert classification)
        {
            var result = await _classificationsService.AddClassificationAsync(classification);

            return result.ResponseCode switch
            {
                HttpStatusCode.OK => Ok(),
                HttpStatusCode.BadRequest => BadRequest(result.Message),
                _ => throw new ArgumentException("Unknown response code")
            };
        }

        /// <summary>
        /// Удаление классификации
        /// </summary>
        /// <param name="classificationId">Id классификации</param>
        /// <returns></returns>
        [HttpDelete("{classificationId:guid}")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Удаление прошло успешно")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Возвращается, если не удалось удалить классификацию", typeof(string))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Произошла неизвестная ошибка")]
        public async Task<IActionResult> DeleteClassifications(Guid classificationId)
        {
            var result = await _classificationsService.DeleteClassificationAsync(classificationId);

            return result.ResponseCode switch
            {
                HttpStatusCode.NoContent => NoContent(),
                HttpStatusCode.BadRequest => BadRequest(result.Message),
                _ => throw new ArgumentException("Unknown response code")
            };
        }

        /// <summary>
        /// Переименование классификации
        /// </summary>
        /// <param name="classificationId">Id классификации</param>
        /// <param name="newName">Новое имя классификации</param>
        /// <returns></returns>
        [HttpPut("{classificationId:guid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Переименование прошло успешно")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Возвращается, когда не удалось переименовать классификацию", typeof(string))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Произошла неизвестная ошибка")]
        public async Task<IActionResult> RenameClassification(Guid classificationId, [FromBody] string newName)
        {
            var result = await _classificationsService.RenameClassificationAsync(classificationId, newName);

            return result.ResponseCode switch
            {
                HttpStatusCode.OK => Ok(),
                HttpStatusCode.BadRequest => BadRequest(result.Message),
                _ => throw new ArgumentException("Unknown response code")
            };
        }

        /// <summary>
        /// Получение числа классификаций по строке
        /// </summary>
        /// <param name="query">Строка</param>
        /// <returns></returns>
        [HttpGet("count")]
        [SwaggerResponse(StatusCodes.Status200OK, "Возвращает число классификаций", typeof(int))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Произошла неизвестная ошибка")]
        public async Task<IActionResult> GetClassificationsCountByQuery([FromQuery] string query)
        {
            var result = await _classificationsService.GetCountClassificationsByQueryAsync(query);

            return result.ResponseCode switch
            {
                HttpStatusCode.OK => Ok(result.Value),
                _ => throw new ArgumentException("Unknown response code")
            };
        }

        /// <summary>
        /// Добавление слова в классификацию
        /// </summary>
        /// <param name="classificationId">Id классификации</param>
        /// <param name="classificationWord">Слово</param>
        /// <returns></returns>
        [HttpPost("{classificationId:guid}/words")]
        [SwaggerResponse(StatusCodes.Status200OK, "Добавление прошло успешно")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Возвращается, если слово в такой квалификации уже существует", typeof(string))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Произошла неизвестная ошибка")]
        public async Task<IActionResult> AddWordToClassification(
            Guid classificationId,
            [FromBody] ClassificationWordInsert classificationWord
        )
        {
            var result = await _classificationsService
                .AddWordToClassificationAsync(classificationId, classificationWord);

            return result.ResponseCode switch
            {
                HttpStatusCode.OK => Ok(),
                HttpStatusCode.BadRequest => BadRequest(result.Message),
                _ => throw new ArgumentException("Unknown response code")
            };
        }

        /// <summary>
        /// Удаления слова из классификации
        /// </summary>
        /// <param name="wordId">Id классификации</param>
        /// <returns></returns>
        [HttpDelete("words/{wordId:guid}")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Удаление прошло успешно")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Возвращается, когда не удалось удалить слово", typeof(string))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Произошла неизвестная ошибка")]
        public async Task<IActionResult> DeleteWordFromClassification(Guid wordId)
        {
            var result = await _classificationsService.DeleteWordAsync(wordId);

            return result.ResponseCode switch
            {
                HttpStatusCode.OK => NoContent(),
                HttpStatusCode.BadRequest => BadRequest(result.Message),
                _ => throw new ArgumentException("Unknown response code")
            };
        }

        /// <summary>
        /// Получение списка слов классификации
        /// </summary>
        /// <param name="classificationId">Id классификации</param> 
        /// <returns></returns>
        [HttpGet("{classificationId:guid}/words")]
        [SwaggerResponse(StatusCodes.Status200OK, "Возвращает список слов классификации", typeof(IEnumerable<ClassificationWord>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Произошла неизвестная ошибка")]
        public async Task<IActionResult> GetWordsByClassificationId(Guid classificationId)
        {
            var result = await _classificationsService.GetWordsByClassificationIdAsync(classificationId);

            return result.ResponseCode switch
            {
                HttpStatusCode.OK => Ok(result.Value),
                _ => throw new ArgumentException("Unknown response code")
            };
        }
    }
}