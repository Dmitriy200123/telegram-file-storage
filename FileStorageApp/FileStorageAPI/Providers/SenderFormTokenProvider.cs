using System;
using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Factories;
using FileStorageApp.Data.InfoStorage.Models;
using Microsoft.AspNetCore.Http;
using RightServices;

namespace FileStorageAPI.Providers
{
    /// <inheritdoc />
    public class SenderFormTokenProvider : ISenderFormTokenProvider
    {
        private readonly IInfoStorageFactory _infoStorageFactory;
        private readonly IUserIdFromTokenProvider _userIdFromTokenProvider;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="SenderFormTokenProvider"/>
        /// </summary>
        /// <param name="infoStorageFactory">Фабрика для создания хранилищ</param>
        /// <param name="userIdFromTokenProvider">Получение идентификатора пользователя из токена</param>
        public SenderFormTokenProvider(IInfoStorageFactory infoStorageFactory,
            IUserIdFromTokenProvider userIdFromTokenProvider)
        {
            _infoStorageFactory = infoStorageFactory;
            _userIdFromTokenProvider = userIdFromTokenProvider;
        }

        /// <inheritdoc />
        public async Task<FileSender?> GetSenderFromToken(HttpRequest request)
        {
            var userId = _userIdFromTokenProvider.GetUserIdFromToken(request, Settings.Key);
            var sendersStorage = _infoStorageFactory.CreateFileSenderStorage();
            var usersStorage = _infoStorageFactory.CreateUsersStorage();
            var user = await usersStorage.GetByIdAsync(userId);
            
            if (user == null)
                throw new InvalidOperationException("User not found");
            
            var sender = await sendersStorage.GetByTelegramIdAsync(user.TelegramId!.Value, true);

            return sender;
        }
    }
}