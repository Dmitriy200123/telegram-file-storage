using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API;
using DocumentClassificationsAPI.Models;

namespace DocumentClassificationsAPI.Services
{
    /// <summary>
    /// Сервис взаимодействия с хранилищем классификаций
    /// </summary>
    public interface IDocumentClassificationsService
    {
        /// <summary>
        /// Получение классификации по Id
        /// </summary>
        /// <param name="id">Id классификации</param>
        /// <param name="includeClassificationWords">Включить в классификацию принадлежащие слова</param>
        /// <returns></returns>
        Task<RequestResult<Classification>> FindClassificationByIdAsync(
            Guid id,
            bool includeClassificationWords
        );

        /// <summary>
        /// Поиск классификаций по строке
        /// </summary>
        /// <param name="query">Строка</param>
        /// <param name="skip">Количество пропускаемых элементов</param>
        /// <param name="take">Количество возвращаемых элементов</param>
        /// <param name="includeClassificationWords">Включить в классификации принадлежащие списки слов</param>
        /// <returns></returns>
        Task<RequestResult<IEnumerable<Classification>>> FindClassificationByQueryAsync(
            string query,
            int skip,
            int take,
            bool includeClassificationWords
        );

        /// <summary>
        /// Добавление классификации
        /// </summary>
        /// <param name="classification">Классификация</param>
        /// <returns></returns>
        Task<RequestResult<bool>> AddClassificationAsync(ClassificationInsert classification);

        /// <summary>
        /// Удаление классификации
        /// </summary>
        /// <param name="id">Id классификации</param>
        /// <returns></returns>
        Task<RequestResult<bool>> DeleteClassificationAsync(Guid id);

        /// <summary>
        /// Переименование классификации
        /// </summary>
        /// <param name="id">Id классификации</param>
        /// <param name="newName">Новое имя классификации</param>
        /// <returns></returns>
        Task<RequestResult<bool>> RenameClassificationAsync(Guid id, string newName);

        /// <summary>
        /// Получение числа классификаций по строке
        /// </summary>
        /// <param name="query">строка</param>
        /// <returns></returns>
        Task<RequestResult<int>> GetCountClassificationsByQueryAsync(string query);

        /// <summary>
        /// Добавление слова в классификацию
        /// </summary>
        /// <param name="classificationId">Id классификации</param>
        /// <param name="classificationWord">Слово</param>
        /// <returns></returns>
        Task<RequestResult<bool>> AddWordToClassificationAsync(
            Guid classificationId,
            ClassificationWordInsert classificationWord
        );

        /// <summary>
        /// Удаления слова из классификации
        /// </summary>
        /// <param name="wordId">Id классификации</param>
        /// <returns></returns>
        Task<RequestResult<bool>> DeleteWordAsync(Guid wordId);

        /// <summary>
        /// Получение списка слов классификации
        /// </summary>
        /// <param name="classificationId">Id классификации</param> 
        /// <returns></returns>
        Task<RequestResult<IEnumerable<ClassificationWord>>> GetWordsByClassificationIdAsync(Guid classificationId);
    }
}