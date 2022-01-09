using System;
using Microsoft.AspNetCore.Http;

namespace FileStorageAPI.Providers
{
    public interface IUserIdFromTokenProvider
    {
        Guid GetUserIdFromToken(HttpRequest request, byte[] key);
    }
}