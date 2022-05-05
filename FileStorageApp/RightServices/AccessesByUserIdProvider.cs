using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Factories;
using FileStorageApp.Data.InfoStorage.Models;

namespace RightServices
{
    /// <inheritdoc />
    public class AccessesByUserIdProvider : IAccessesByUserIdProvider
    {
        private readonly IInfoStorageFactory _infoStorageFactory;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="AccessesByUserIdProvider"/>.
        /// </summary>
        /// <param name="infoStorageFactory">Фабрика для получения доступа к базе данных пользователей</param>
        public AccessesByUserIdProvider(IInfoStorageFactory infoStorageFactory)
        {
            _infoStorageFactory = infoStorageFactory ?? 
                                  throw new ArgumentNullException(nameof(infoStorageFactory));
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Accesses>> GetAccessesByUserIdAsync(Guid userId)
        {
            using var usersStorage = _infoStorageFactory.CreateUsersStorage();
            var user = await usersStorage.GetByIdAsync(userId, true);
            
            if (user == null)
                throw new InvalidOperationException("User not found");
            
            return user.Rights.Select(right => right.AccessType);
        }
    }
}