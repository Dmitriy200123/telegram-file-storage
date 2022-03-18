using System;
using System.Linq;
using System.Threading.Tasks;
using FileStorageAPI.Providers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FileStorageAPI.RightsFilters
{
    internal class RightsFilter : IRightsFilter
    {
        private readonly IAccessesByUserIdProvider _accessesByUserIdProvider;
        private readonly IUserIdFromTokenProvider _userIdFromTokenProvider;

        public RightsFilter(
            IAccessesByUserIdProvider accessesByUserIdProvider,
            IUserIdFromTokenProvider userIdFromTokenProvider
        )
        {
            _accessesByUserIdProvider = accessesByUserIdProvider ??
                                        throw new ArgumentNullException(nameof(accessesByUserIdProvider));
            _userIdFromTokenProvider = userIdFromTokenProvider ??
                                       throw new ArgumentNullException(nameof(userIdFromTokenProvider));
        }

        public async Task<bool> CheckRightsAsync(ActionExecutingContext filterContext, int[] accesses)
        {
            var userId = _userIdFromTokenProvider.GetUserIdFromToken(filterContext.HttpContext.Request, Settings.Key);
            var userAccesses = (await _accessesByUserIdProvider.GetAccessesByUserIdAsync(userId))
                .Cast<int>();
            var accessIntersections = userAccesses.Intersect(accesses);

            return accesses.Length == accessIntersections.Count();
        }
    }
}