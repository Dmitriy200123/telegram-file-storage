using System;
using FileStorageApp.Data.InfoStorage.Models;

namespace FileStorageApp.Data.InfoStorage.Tables.Chats
{
    public interface IChatTable
    {
        Chat GetChatById(Guid id);
    }
}