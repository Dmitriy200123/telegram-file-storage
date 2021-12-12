using System;
using FileStorageApp.Data.InfoStorage.Models;

namespace FileStorageApp.Data.InfoStorage.Storages.Users
{
    public interface IUsersStorage : IDisposable, IInfoStorage<User>
    {
        
    }
}