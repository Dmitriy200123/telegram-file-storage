using System;
using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Models;

namespace FileStorageApp.Data.InfoStorage.Storages.AccessStorage
{
    public interface IRightsStorage : IDisposable, IInfoStorage<Right>
    {
        Task<int[]> GetUserRightsAsync(Guid id);
        Task<bool> RemoveRightAsync(Guid id, int access);
        Task<Right[]> GetAllAsync();
    }
}