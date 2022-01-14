using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FileStorageApp.Data.InfoStorage.Models;

namespace FileStorageAPI.Providers
{
    /// <inheritdoc />
    public class ExpressionFileFilterProvider : IExpressionFileFilterProvider
    {
        /// <inheritdoc />
        public Expression<Func<File, bool>> GetExpression(FileSearchParameters parameters, List<Guid>? chatsId = null)
        {
            var categories = parameters.Categories?.Cast<int>().ToList();
            return x => (parameters.FileName == null || x.Name == parameters.FileName) &&
                        (parameters.DateFrom == null || parameters.DateFrom <= x.UploadDate) &&
                        (parameters.DateTo == null || parameters.DateTo >= x.UploadDate) &&
                        (categories == null || categories.Contains(x.TypeId)) &&
                        (chatsId == null || chatsId.Contains(x.ChatId!.Value) || x.ChatId == null) &&
                        (parameters.SenderIds == null || parameters.SenderIds.Contains(x.FileSenderId)) &&
                        (parameters.ChatIds == null || x.ChatId != null && parameters.ChatIds.Contains(x.ChatId.Value) ||
                        x.ChatId == null && parameters.ChatIds.Contains(Guid.Empty));
        }
    }
}