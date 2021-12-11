using System;
using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Config;
using FileStorageApp.Data.InfoStorage.Models;
using Microsoft.EntityFrameworkCore;

namespace FileStorageApp.Data.InfoStorage.Storages.Users
{
    internal class UsersStorage : BaseStorage<User>, IUsersStorage
    {
        internal UsersStorage(IDataBaseConfig dataBaseConfig) : base(dataBaseConfig)
        {
        }

        public async Task<bool> AddTelegramIdToUser(Guid id, long telegramId)
        {
            var user = await DbSet.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
                return false;
            user.TelegramId = telegramId;
            await SaveChangesAsync();
            return true;
        }

        public async Task<bool> HasTelegramId(long telegramId)
        {
            var user = await DbSet.FirstOrDefaultAsync(x => x.TelegramId == telegramId);
            return user is not null;
        }

        public async Task<bool> IsRegisteredAsync(int gitLabId)
        {
            var user = await DbSet.FirstOrDefaultAsync(x => x.GitLabId == gitLabId);
            return user is not null;
        }

        public async Task<User> GetByGitLabIdAsync(int gitLabId)
        {
            return await DbSet.FirstOrDefaultAsync(x => x.GitLabId == gitLabId);
        }
    }
}