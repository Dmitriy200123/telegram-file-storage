using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FileStorageApp.Data.InfoStorage.Storages.Chats
{
    public interface IChatStorage : IDisposable
    {
        public Task<List<Chat>> GetAll();
        public Task<Chat> GetById(Guid id);
        public Task<List<Chat>> GetBySubString(string subString);
        public Task<bool> Add(Chat chat);
        public Task<bool> Update(Chat chat);
        public Task<bool> Delete(Guid id);
        public Task<bool> Contains(Guid id);
    }
}