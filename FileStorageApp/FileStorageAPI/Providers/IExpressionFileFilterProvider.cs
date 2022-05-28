using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FileStorageAPI.Models;
using FileStorageApp.Data.InfoStorage.Models;

namespace FileStorageAPI.Providers
{
    /// <summary>
    /// Поставщик query Expression для поиска файлов.
    /// </summary>
    public interface IExpressionFileFilterProvider
    {
        /// <summary>
        /// Возвращает query Expression.
        /// </summary>
        /// <param name="parameters">Параметры поиска файлов</param>
        /// <param name="chatsId"></param>
        Expression<Func<File, bool>> GetExpression(FileSearchParameters parameters, List<Guid>? chatsId = null);

        /// <summary>
        /// Возвращает query Expression для документа
        /// </summary>
        /// <param name="parameters">параметры поиска файла</param>
        /// <param name="fileIds">Идентификаторы документов среди которых нужно искать</param>
        /// <param name="chatsId">Идентификаторы чатов, к которым пользователь имеет доступ</param>
        /// <returns></returns>
        Expression<Func<File, bool>> GetDocumentExpression(DocumentSearchParameters parameters,
            List<Guid>? fileIds,
            List<Guid>? chatsId = null);
    }
}