using System;
using System.Linq.Expressions;
using FileStorageApp.Data.InfoStorage.Models;

namespace FileStorageAPI.Providers
{
    public interface IExpressionFileFilterProvider
    {
        Expression<Func<File, bool>> GetExpression(FileSearchParameters parameters);
    }
}