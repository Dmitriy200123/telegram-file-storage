using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Factories;
using JwtAuth;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;

namespace FileStorageAPI.RightsFilters
{
    internal class RightsFilter : IRightsFilter
    {
        private readonly IInfoStorageFactory _infoStorageFactory;

        public RightsFilter(IInfoStorageFactory infoStorageFactory)
        {
            _infoStorageFactory = infoStorageFactory;
        }

        public async Task<bool> CheckRightsAsync(ActionExecutingContext filterContext, int[] accesses)
        {
            var authHeader = filterContext.HttpContext.Request.Headers[HeaderNames.Authorization];
            var userToken = authHeader.ToString().Split(' ')[1];
            var principal = TokenHelper.GetPrincipalFromToken(userToken, Settings.Key);
            var username = principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name);
            var userId = Guid.Parse(username!.Value);

            using var userStorage = _infoStorageFactory.CreateUsersStorage();
            var user = await userStorage.GetByIdAsync(userId, true);
            var userAccesses = user!.Rights.Select(right => right.Access);
            var accessIntersections = userAccesses.Intersect(accesses);

            return accesses.Length == accessIntersections.Count();
        }
    }
}