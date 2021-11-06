using System;
using System.Collections.Generic;
using System.Linq;
using FileStorageApp.Data.InfoStorage.Storages.Chats;
using FileStorageApp.Data.InfoStorage.Factories;
using FileStorageApp.Data.InfoStorage.Config;
using System.Threading.Tasks;
using FileStorageAPI.Models;

namespace FileStorageAPI.Services
{
    public class ChatClient
    {
        private IChatStorage _chatClient;

        public ChatClient(IDataBaseConfig config)
        {
            _chatClient = new InfoStorageFactory(config).CreateChatStorage();
        }

        public async Task<Chat> GetChat(Guid id)
        {
            var chats = await _chatClient.GetAllAsync();
            var chat = chats.Find(x => x.Id == id);

            return chat != null ? new Chat(chat.Id, chat.ImageId, chat.Name) : null;
        }

        public async Task<List<Chat>> SearchChats(string chatName)
        {
            var chat = await _chatClient.GetByChatNameSubstringAsync(chatName);

            var result = chat.Select(x => new Chat(x.Id, x.ImageId, x.Name));

            return result.ToList();
        }
    }
}