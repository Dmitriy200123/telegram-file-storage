using System;
using JwtAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace FileStorageAPI.Providers
{
    public class UserIdFromTokenProvider : IUserIdFromTokenProvider
    {
        public Guid GetUserIdFromToken(HttpRequest request, byte[] key)
        {
            var authHeader = request.Headers[HeaderNames.Authorization];
            var userToken = authHeader.ToString().Split(' ')[1];
            var principal = TokenHelper.GetPrincipalFromToken(userToken, key);

            var userName = principal.Identity!.Name;
            return Guid.Parse(userName!);
        }
    }
}