using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentsIndex.Contracts;

namespace DocumentsIndex
{
    /// <summary>
    /// Хранилище текстовых файлов с поиском по содержимому
    /// </summary>
    public interface IDocumentIndexStorage
    {
        /// <summary>
        /// Поиск документов, в которых присутствует заданная подстрока
        /// </summary>
        /// <param name="subString">подстрока по которой происходит поиск</param>
        /// <returns></returns>
        Task<List<Guid>> SearchBySubstringAsync(string subString);

        /// <summary>
        /// Поиск документов по названию
        /// </summary>
        /// <param name="name">имя документа, которое необходимо найти</param>
        /// <returns></returns>
        Task<List<Guid>> SearchByNameAsync(string name);

        /// <summary>
        /// Индексация (добавление) документа в эластик
        /// </summary>
        /// <param name="document">документ, который необходимо добавить</param>
        /// <returns></returns>
        Task<bool> IndexDocumentAsync(Document document);

        /// <summary>
        /// Удаление документа из эластика
        /// </summary>
        /// <param name="guid">Идентификатор документа, который нужно удалить</param>
        /// <returns></returns>
        Task<bool> DeleteAsync(Guid guid);

        /// <summary>
        /// Поиск подстроки в тексте или имени файла
        /// </summary>
        /// <param name="query">подстрока для поиска</param>
        /// <returns></returns>
        Task<List<Guid>> FindInTextOrNameAsync(string query);

        /// <summary>
        /// Проверки содержания подстрок в названии документа с заданным идентификатором
        /// </summary>
        /// <param name="documentId">Идентификатор документа</param>
        /// <param name="subStrings">Подстроки для поиска</param>
        /// <returns></returns>
        Task<bool> IsContainsInNameAsync(Guid documentId, string[] subStrings);
    }
}