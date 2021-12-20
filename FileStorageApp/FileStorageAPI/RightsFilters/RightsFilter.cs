using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using JwtAuth;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace FileStorageAPI.RightsFilters
{
    internal class RightsFilter : IRightsFilter
    {
        public bool CheckRights(ActionExecutingContext filterContext, IEnumerable<int> accesses)
        {
            var authHeader = filterContext.HttpContext.Request.Headers[HeaderNames.Authorization];
            var userToken = authHeader.ToString().Split(' ')[1];
            var principal = TokenHelper.GetPrincipalFromToken(userToken, Settings.Key);
            var accessClaims = principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role);
            if (accessClaims is null)
                return false;

            var accessArray = JsonConvert.DeserializeObject<List<int>>(accessClaims!.Value);
            return accessArray is not null && accessArray.Intersect(accesses).Any();
        }
    }
}