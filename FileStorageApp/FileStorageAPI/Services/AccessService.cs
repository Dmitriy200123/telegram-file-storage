using System.Linq;
using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Models;
using Microsoft.AspNetCore.Http;
using RightServices;

namespace FileStorageAPI.Services
{
    public class AccessService : IAccessService
    {
        private readonly IUserIdFromTokenProvider _userIdFromTokenProvider;
        private readonly IAccessesByUserIdProvider _accessesByUserIdProvider;
        
        public AccessService(IUserIdFromTokenProvider userIdFromTokenProvider, IAccessesByUserIdProvider accessesByUserIdProvider)
        {
            _userIdFromTokenProvider = userIdFromTokenProvider;
            _accessesByUserIdProvider = accessesByUserIdProvider;
        }
        
        public async Task<bool> HasAnyFilesAccessAsync(HttpRequest request)
        {
            var userId = _userIdFromTokenProvider.GetUserIdFromToken(request, Settings.Key);
            var accesses = await _accessesByUserIdProvider.GetAccessesByUserIdAsync(userId);
            return accesses.Any(access => access == Accesses.ViewAnyFiles);
        }
    }
}