using System;
using System.Collections.Generic;
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
            Database.EnsureCreated();
        }

        public async Task<bool> AddTelegramIdToGitLabUserAsync(long id, long telegramId)
        {
            var user = await DbSet.FirstOrDefaultAsync(x => x.GitLabId == id);
            if (user == null)
                return false;
            user.TelegramId = telegramId;
            await SaveChangesAsync();
            return true;
        }

        public async Task<bool> HasTelegramIdAsync(long telegramId)
        {
            var user = await DbSet.FirstOrDefaultAsync(x => x.TelegramId == telegramId);
            return user is not null;
        }

        public async Task<bool> IsRegisteredAsync(int gitLabId)
        {
            var user = await DbSet.FirstOrDefaultAsync(x => x.GitLabId == gitLabId);
            return user is not null;
        }

        public async Task<User?> GetByGitLabIdAsync(int gitLabId)
        {
            return await DbSet.Include(x => x.Rights).FirstOrDefaultAsync(x => x.GitLabId == gitLabId);
        }

        public async Task<bool> UpdateRefreshTokenAsync(Guid id, string refreshToken)
        {
            var user = await DbSet.FirstOrDefaultAsync(x => x.Id == id);
            if (user is null)
                return false;
            user.RefreshToken = refreshToken;
            await SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveRefreshTokenAsync(Guid id)
        {
            var user = await DbSet.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
                return false;
            user.RefreshToken = "";
            await SaveChangesAsync();
            return true;
        }

        public async Task<string?> GetRefreshTokenAsync(Guid id)
        {
            var user = await DbSet.FirstOrDefaultAsync(x => x.Id == id);
            return user?.RefreshToken;
        }

        public new async Task<User?> GetByIdAsync(Guid id)
        {
            return await DbSet.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<User>> GetAll()
        {
            return await DbSet.ToListAsync();
        }
    }
}