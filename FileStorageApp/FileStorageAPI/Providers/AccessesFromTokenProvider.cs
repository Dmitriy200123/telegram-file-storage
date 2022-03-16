using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Factories;
using FileStorageApp.Data.InfoStorage.Models;
using Microsoft.AspNetCore.Http;

namespace FileStorageAPI.Providers
{
    /// <inheritdoc />
    public class AccessesFromTokenProvider : IAccessesFromTokenProvider
    {
        private readonly IUserIdFromTokenProvider _userIdFromTokenProvider;
        private readonly IInfoStorageFactory _infoStorageFactory;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="AccessesFromTokenProvider"/>.
        /// </summary>
        /// <param name="userIdFromTokenProvider">Провайдер для получения Id пользователя из токена</param>
        /// <param name="infoStorageFactory">Фабрика для получения доступа к базе данных пользователей</param>
        public AccessesFromTokenProvider(IUserIdFromTokenProvider userIdFromTokenProvider, IInfoStorageFactory infoStorageFactory)
        {
            _userIdFromTokenProvider = userIdFromTokenProvider ?? 
                                       throw new ArgumentNullException(nameof(userIdFromTokenProvider));
            _infoStorageFactory = infoStorageFactory ?? 
                                  throw new ArgumentNullException(nameof(infoStorageFactory));
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Accesses>> GetAccessesFromTokenAsync(HttpRequest request)
        {
            var userId = _userIdFromTokenProvider.GetUserIdFromToken(request, Settings.Key);

            using var usersStorage = _infoStorageFactory.CreateUsersStorage();
            var user = await usersStorage.GetByIdAsync(userId, true);
            
            if (user == null)
                throw new InvalidOperationException("User not found");
            
            return user.Rights.Select(right => right.AccessType);
        }
    }
}