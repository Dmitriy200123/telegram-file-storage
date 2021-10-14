using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace FileStorageApp.Data.InfoStorage.Tables.Chat
{
    internal class ChatTable : IChatTable
    {
        private IMainStorage _mainStorage;
        public ChatTable(IMainStorage mainStorage)
        {
            this._mainStorage = mainStorage;
        }

        public Models.Chat GetChatById(Guid id)
        {
            var fileSender = _mainStorage.Chats.FirstOrDefault(x => x.Id == id);
            if (fileSender == null)
                throw new ArgumentException($"Chat with id {id} not found");
            return fileSender;
        }
    }
}