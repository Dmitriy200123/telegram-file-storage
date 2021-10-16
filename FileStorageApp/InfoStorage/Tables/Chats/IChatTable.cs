using System;
using FileStorageApp.Data.InfoStorage.Models;

namespace FileStorageApp.Data.InfoStorage.Tables.Chats
{
    public interface IChatTable : IDisposable
    {
        Chat GetChatById(Guid id);
    }
}