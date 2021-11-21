using System;
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
        Expression<Func<File, bool>> GetExpression(FileSearchParameters parameters);
    }
}