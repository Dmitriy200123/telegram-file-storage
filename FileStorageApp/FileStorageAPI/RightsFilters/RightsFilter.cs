using System;
using System.Linq;
using System.Threading.Tasks;
using FileStorageAPI.Providers;
using FileStorageApp.Data.InfoStorage.Factories;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FileStorageAPI.RightsFilters
{
    internal class RightsFilter : IRightsFilter
    {
        private readonly IInfoStorageFactory _infoStorageFactory;
        private readonly IUserIdFromTokenProvider _userIdFromTokenProvider;

        public RightsFilter(IInfoStorageFactory infoStorageFactory, IUserIdFromTokenProvider userIdFromTokenProvider)
        {
            _infoStorageFactory = infoStorageFactory ?? throw new ArgumentNullException(nameof(infoStorageFactory));
            _userIdFromTokenProvider = userIdFromTokenProvider ?? throw new ArgumentNullException(nameof(userIdFromTokenProvider));
        }

        public async Task<bool> CheckRightsAsync(ActionExecutingContext filterContext, int[] accesses)
        {
            var userId = _userIdFromTokenProvider.GetUserIdFromToken(filterContext.HttpContext.Request, Settings.Key);
            using var userStorage = _infoStorageFactory.CreateUsersStorage();
            var user = await userStorage.GetByIdAsync(userId, true);
            if (user == null)
                throw new InvalidOperationException("User not found");

            var userAccesses = user.Rights.Select(right => right.Access);
            var accessIntersections = userAccesses.Intersect(accesses);

            return accesses.Length == accessIntersections.Count();
        }
    }
}