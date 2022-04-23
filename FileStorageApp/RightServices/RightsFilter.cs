using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RightServices
{
    public class RightsFilter : IRightsFilter
    {
        private readonly IAccessesByUserIdProvider _accessesByUserIdProvider;
        private readonly IUserIdFromTokenProvider _userIdFromTokenProvider;
        private readonly RightsSettings _rightsSettings;

        public RightsFilter(
            IAccessesByUserIdProvider accessesByUserIdProvider,
            IUserIdFromTokenProvider userIdFromTokenProvider, RightsSettings rightsSettings)
        {
            _accessesByUserIdProvider = accessesByUserIdProvider ??
                                        throw new ArgumentNullException(nameof(accessesByUserIdProvider));
            _userIdFromTokenProvider = userIdFromTokenProvider ??
                                       throw new ArgumentNullException(nameof(userIdFromTokenProvider));
            _rightsSettings = rightsSettings ?? throw new ArgumentNullException(nameof(rightsSettings));
        }

        public async Task<bool> CheckRightsAsync(ActionExecutingContext filterContext, int[] accesses)
        {
            var userId = _userIdFromTokenProvider.GetUserIdFromToken(filterContext.HttpContext.Request, _rightsSettings.Key);
            var userAccesses = (await _accessesByUserIdProvider.GetAccessesByUserIdAsync(userId))
                .Cast<int>();
            var accessIntersections = userAccesses.Intersect(accesses);

            return accesses.Length == accessIntersections.Count();
        }
    }
}