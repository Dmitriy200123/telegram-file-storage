using System;
using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Models;

namespace FileStorageApp.Data.InfoStorage.Storages.Users
{
    public interface IUsersStorage : IDisposable, IInfoStorage<User>
    {
        Task<bool> AddTelegramIdToUser(Guid id, long telegramId);
        Task<bool> HasTelegramId(long telegramId);
        Task<bool> IsRegisteredAsync(int gitLabId);
        Task<User> GetByGitLabIdAsync(int gitLabId);
    }
}