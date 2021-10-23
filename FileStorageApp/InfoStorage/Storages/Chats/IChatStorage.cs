using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Models;

namespace FileStorageApp.Data.InfoStorage.Storages.Chats
{
    public interface IChatStorage : IDisposable, IInfoStorage<Chat>
    {
        public Task<List<Chat>> GetBySubStringAsync(string subString);
    }
}