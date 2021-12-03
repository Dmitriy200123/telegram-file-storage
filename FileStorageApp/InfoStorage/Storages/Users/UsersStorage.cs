using FileStorageApp.Data.InfoStorage.Config;
using FileStorageApp.Data.InfoStorage.Models;

namespace FileStorageApp.Data.InfoStorage.Storages.Users
{
    internal class UsersStorage : BaseStorage<User>, IUsersStorage
    {
        internal UsersStorage(IDataBaseConfig dataBaseConfig) : base(dataBaseConfig)
        {
        }
    }
}