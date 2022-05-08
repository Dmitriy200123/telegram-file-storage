using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FileStorageAPI.Models;
using Microsoft.AspNetCore.Http;
using DataBaseFile = FileStorageApp.Data.InfoStorage.Models.File;

namespace FileStorageAPI.Providers
{
    public interface IDocumentsProvider
    {
        Task<int> GetDocumentsCountByParametersAndIds(FileSearchParameters fileSearchParameters,
            List<Guid>? guidsToFind, HttpRequest request);

        Task<List<DocumentInfo>> GetDocumentsByParametersAndIds(
            FileSearchParameters fileSearchParameters, List<Guid>? fileIds, HttpRequest request, int skip, int take);

        Task<List<FileInfo>> GetFileInfoFromStorage(Expression<Func<DataBaseFile, bool>> expression,
            int? skip, int? take);
    }
}