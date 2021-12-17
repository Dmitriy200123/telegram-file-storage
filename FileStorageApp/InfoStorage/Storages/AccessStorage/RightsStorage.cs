using System;
using System.Linq;
using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Config;
using FileStorageApp.Data.InfoStorage.Models;
using Microsoft.EntityFrameworkCore;

namespace FileStorageApp.Data.InfoStorage.Storages.AccessStorage
{
    internal class RightsStorage : BaseStorage<Right>, IRightsStorage
    {
        internal RightsStorage(IDataBaseConfig dataBaseConfig) : base(dataBaseConfig)
        {
        }

        public async Task<int[]> GetUserRights(Guid id)
        {
            return await DbSet.Where(x => x.Id == id).Select(x => x.Access).ToArrayAsync();
        }

        public async Task<bool> RemoveRight(Guid id, int access)
        {
            var right = await DbSet.FirstOrDefaultAsync(x => x.Id == id && x.Access == access);
            if (right is null)
                return false;
            DbSet.Remove(right);
            await SaveChangesAsync();
            return true;
        }
    }
}