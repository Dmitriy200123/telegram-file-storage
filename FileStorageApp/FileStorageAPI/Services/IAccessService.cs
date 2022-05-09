using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FileStorageAPI.Services
{
    public interface IAccessService
    {
        Task<bool> HasAnyFilesAccessAsync(HttpRequest request);
    }
}