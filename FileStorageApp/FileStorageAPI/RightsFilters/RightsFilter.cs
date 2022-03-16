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
        private readonly IAccessesFromTokenProvider _accessesFromTokenProvider;

        public RightsFilter(IInfoStorageFactory infoStorageFactory,
            IAccessesFromTokenProvider accessesFromTokenProvider)
        {
            _infoStorageFactory = infoStorageFactory ?? throw new ArgumentNullException(nameof(infoStorageFactory));
            _accessesFromTokenProvider = accessesFromTokenProvider ??
                                         throw new ArgumentNullException(nameof(accessesFromTokenProvider));
        }

        public async Task<bool> CheckRightsAsync(ActionExecutingContext filterContext, int[] accesses)
        {
            var userAccesses = (await _accessesFromTokenProvider
                .GetAccessesFromTokenAsync(filterContext.HttpContext.Request))
                .Cast<int>();
            var accessIntersections = userAccesses.Intersect(accesses);

            return accesses.Length == accessIntersections.Count();
        }
    }
}