using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace FileStorageApp.Data.InfoStorage.Tables.Chat
{
    public class ChatTable : IChatTable
    {
        private DbSet<Models.Chat> _сhats { get; set; }
        internal ChatTable(IMainStorage mainStorage)
        {
            _сhats = mainStorage.Chats;
        }

        public Models.Chat GetChatById(Guid id)
        {
            var fileSender = _сhats.FirstOrDefault(x => x.Id == id);
            if (fileSender == null)
                throw new ArgumentException($"Chat with id {id} not found");
            return fileSender;
        }
    }
}