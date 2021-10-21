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
        public ValueTask<EntityEntry<Chat>> Add(Chat chat);
        public Task<EntityEntry<Chat>> Update(Guid id, string? name = null, Guid? imageId = null);
        public Task<EntityEntry<Chat>> Delete(Guid id);
        public Task<bool> Contains(Guid id);
    }
}