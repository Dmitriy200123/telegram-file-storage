using System;
using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Models;

namespace FileStorageApp.Data.InfoStorage.Storages.Users
{
    public interface IUsersStorage : IDisposable, IInfoStorage<User>
    {
        Task<bool> AddTelegramIdToGitLabUserAsync(long id, long telegramId);
        Task<bool> HasTelegramIdAsync(long telegramId);
        Task<bool> IsRegisteredAsync(int gitLabId);
        Task<User?> GetByGitLabIdAsync(int gitLabId);
        Task<bool> UpdateRefreshTokenAsync(Guid id, string refreshToken);
        Task<bool> RemoveRefreshTokenAsync(Guid id);
        Task<string?> GetRefreshTokenAsync(Guid id);
        Task<User?> GetByIdAsync(Guid id);
    }
}