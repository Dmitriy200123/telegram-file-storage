using System;

namespace FileStorageApp.Data.InfoStorage.Tables.Chat
{
    public interface IChatTable
    {
        Models.Chat GetChatById(Guid id);
    }
}