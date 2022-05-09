using System.Linq;
using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Models;
using Microsoft.AspNetCore.Http;
using RightServices;

namespace FileStorageAPI.Services
{
    /// <inheritdoc />
    public class AccessService : IAccessService
    {
        private readonly IUserIdFromTokenProvider _userIdFromTokenProvider;
        private readonly IAccessesByUserIdProvider _accessesByUserIdProvider;
        
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="AccessService"/>
        /// </summary>
        /// <param name="userIdFromTokenProvider">Поставщик идентификатора пользователя из токена</param>
        /// <param name="accessesByUserIdProvider">Поставщик прав пользователя по его идентификатору</param>
        public AccessService(IUserIdFromTokenProvider userIdFromTokenProvider, IAccessesByUserIdProvider accessesByUserIdProvider)
        {
            _userIdFromTokenProvider = userIdFromTokenProvider;
            _accessesByUserIdProvider = accessesByUserIdProvider;
        }

        /// <inheritdoc />
        public async Task<bool> HasAnyFilesAccessAsync(HttpRequest request)
        {
            var userId = _userIdFromTokenProvider.GetUserIdFromToken(request, Settings.Key);
            var accesses = await _accessesByUserIdProvider.GetAccessesByUserIdAsync(userId);
            return accesses.Any(access => access == Accesses.ViewAnyFiles);
        }
    }
}