using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Factories;
using FileStorageApp.Data.InfoStorage.Models;
using JwtAuth;
using Microsoft.AspNetCore.Http;

namespace FileStorageAPI.Providers
{
    /// <inheritdoc />
    public class SenderFormTokenProvider : ISenderFormTokenProvider
    {
        private readonly IInfoStorageFactory _infoStorageFactory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoStorageFactory"></param>
        public SenderFormTokenProvider(IInfoStorageFactory infoStorageFactory)
        {
            _infoStorageFactory = infoStorageFactory;
        }

        /// <inheritdoc />
        public async Task<FileSender?> GetSenderFromToken(HttpRequest request)
        {
            var userId = request.GetUserIdFromToken(Settings.Key);
            var sendersStorage = _infoStorageFactory.CreateFileSenderStorage();
            var usersStorage = _infoStorageFactory.CreateUsersStorage();
            var user = await usersStorage.GetByIdAsync(userId);
            var sender = await sendersStorage.GetByTelegramIdAsync(user!.TelegramId!.Value);
            return sender;
        }
    }
}