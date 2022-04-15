using System;
using System.Security.Claims;
using JwtAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace FileStorageAPI.Providers
{
    /// <inheritdoc />
    public class UserIdFromTokenProvider : IUserIdFromTokenProvider
    {
        /// <inheritdoc />
        public Guid GetUserIdFromToken(HttpRequest request, byte[] tokenKey)
        {
            var authHeader = request.Headers[HeaderNames.Authorization];
            var userToken = authHeader.ToString().Split(' ')[1];
            var principal = TokenHelper.GetPrincipalFromToken(userToken, tokenKey);

            var username = principal.Identity?.Name;
            if (username == null)
                throw new InvalidOperationException($"Doesn't contain claim of {nameof(ClaimTypes.Name)}");
            if (!Guid.TryParse(username, out var userId))
                throw new InvalidOperationException($"Can't convert ${nameof(username)} claim to user id");

            return userId;
        }
    }
}