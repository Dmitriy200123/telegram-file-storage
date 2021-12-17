using System;
using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Models;

namespace FileStorageApp.Data.InfoStorage.Storages.AccessStorage
{
    public interface IRightsStorage : IDisposable, IInfoStorage<Right>
    {
        Task<int[]> GetUserRights(Guid id);
        Task<bool> RemoveRight(Guid id, int access);
    }
}