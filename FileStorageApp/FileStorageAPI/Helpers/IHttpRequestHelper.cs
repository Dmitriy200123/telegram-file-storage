using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Models;
using Microsoft.AspNetCore.Http;

namespace FileStorageAPI.Helpers
{
    public interface IHttpRequestHelper
    {
        Task<FileSender> GetNotNullSenderAsync(HttpRequest request);
        Task<bool> HasAnyFilesAccessAsync(HttpRequest request);
        Task<List<Guid>?> GetUserChats(HttpRequest request);
    }
}