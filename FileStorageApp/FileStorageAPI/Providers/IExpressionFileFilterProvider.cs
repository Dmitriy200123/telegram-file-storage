using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
    }
}