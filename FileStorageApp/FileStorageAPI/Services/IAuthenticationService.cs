using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace FileStorageAPI.Services
{
    public interface IAuthenticationService
    {
        Task<RequestResult<AuthenticationProperties>> LogIn(string? remoteError);
    }
}