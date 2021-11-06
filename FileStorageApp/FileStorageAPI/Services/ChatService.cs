using System;
using System.Collections.Generic;
using System.Linq;
using FileStorageApp.Data.InfoStorage.Factories;
using System.Threading.Tasks;
using FileStorageAPI.Converters;
using FileStorageAPI.Models;

namespace FileStorageAPI.Services
{
    public class ChatService : IChatService
    {
        private readonly IInfoStorageFactory _infoStorageFactory;
        private readonly IChatConverter _chatConverter;

        public ChatService(IInfoStorageFactory infoStorageFactory, IChatConverter chatConverter)
        {
            _infoStorageFactory = infoStorageFactory ?? throw new ArgumentNullException(nameof(infoStorageFactory));
            _chatConverter = chatConverter ?? throw new ArgumentNullException(nameof(chatConverter));
        }

        public async Task<Chat> GetChatByIdAsync(Guid id)
        {
            using var chatStorage = _infoStorageFactory.CreateChatStorage();
            var chatInDb = await chatStorage.GetByIdAsync(id);

            return chatInDb is null ? null : _chatConverter.ConvertToChatInApi(chatInDb);
        }

        public async Task<List<Chat>> GetByChatNameSubstringAsync(string chatName)
        {
            using var chatStorage = _infoStorageFactory.CreateChatStorage();
            var chatsInDb = await chatStorage.GetByChatNameSubstringAsync(chatName);

            return chatsInDb.Select(_chatConverter.ConvertToChatInApi).ToList();
        }
    }
}