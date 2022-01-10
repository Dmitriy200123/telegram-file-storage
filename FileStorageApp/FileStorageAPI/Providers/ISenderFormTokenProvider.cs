using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Models;
using Microsoft.AspNetCore.Http;

namespace FileStorageAPI.Providers
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISenderFormTokenProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<FileSender?> GetSenderFromToken(HttpRequest request);
    }
}