using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FileStorageAPI.Converters;
using FileStorageAPI.Models;
using Microsoft.AspNetCore.Authentication;

namespace FileStorageAPI.Services
{
    public interface IAuthenticationService
    {
        Task<RequestResult<AuthenticationProperties>> LogIn(string? remoteError);
    }
}