using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace FileStorageAPI
{
    public static class ClaimsExtension
    {
        public static string Get(this IEnumerable<Claim> claims, string name)
        {
            return claims.First(x => x.Type == name).Value;
        }
    }
}