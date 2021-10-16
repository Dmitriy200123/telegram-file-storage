using System;
using FileStorageApp.Data.InfoStorage.Models;

namespace FileStorageApp.Data.InfoStorage.Storages.Chats
{
    public interface IChatStorage : IDisposable
    {
        Chat GetChatById(Guid id);
    }
}