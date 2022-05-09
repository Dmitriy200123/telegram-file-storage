using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Models;
using Microsoft.AspNetCore.Http;

namespace FileStorageAPI.Providers
{
    /// <summary>
    /// Поставщик отправителя из токена
    /// </summary>
    public interface ISenderFormTokenProvider
    {
        /// <summary>
        /// Возвращает отправителя из токена
        /// </summary>
        /// <param name="request">Запрос который пришел</param>
        /// <returns></returns>
        Task<FileSender?> GetSenderFromToken(HttpRequest request);
    }
}