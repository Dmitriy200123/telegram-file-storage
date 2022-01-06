using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Factories;
using FileStorageApp.Data.InfoStorage.Models;
using JwtAuth;
using Microsoft.AspNetCore.Http;

namespace FileStorageAPI.Extensions
{
    public static class TokenExtension
    {
        public static async Task<FileSender?> GetSenderFromToken(this HttpRequest request, IInfoStorageFactory infoStorageFactory)
        {
            var userId = request.GetUserIdFromToken(Settings.Key);
            var sendersStorage = infoStorageFactory.CreateFileSenderStorage();
            var usersStorage = infoStorageFactory.CreateUsersStorage();
            var user = await usersStorage.GetByIdAsync(userId);
            var sender = await sendersStorage.GetByTelegramIdAsync(user!.TelegramId!.Value);
            return sender;
        }
    }
}