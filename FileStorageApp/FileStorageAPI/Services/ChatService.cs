using System;
using System.Collections.Generic;
using System.Linq;
using FileStorageApp.Data.InfoStorage.Factories;
using System.Threading.Tasks;
using FileStorageAPI.Converters;
using FileStorageAPI.Models;

namespace FileStorageAPI.Services
{
    /// <inheritdoc/>
    public class ChatService : IChatService
    {
        private readonly IInfoStorageFactory _infoStorageFactory;
        private readonly IChatConverter _chatConverter;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="ChatService"/>
        /// </summary>
        /// <param name="infoStorageFactory">Фабрика для получения доступа к хранилищу чатов</param>
        /// <param name="chatConverter">Конвертор для преобразования чатов в API-контракты</param>>
        public ChatService(IInfoStorageFactory infoStorageFactory, IChatConverter chatConverter)
        {
            _infoStorageFactory = infoStorageFactory ?? throw new ArgumentNullException(nameof(infoStorageFactory));
            _chatConverter = chatConverter ?? throw new ArgumentNullException(nameof(chatConverter));
        }

        /// <inheritdoc/>
        public async Task<RequestResult<List<Chat>>> GetAllChats()
        {
            using var chatStorage = _infoStorageFactory.CreateChatStorage();
            var chatsInDb = await chatStorage.GetAllAsync();

            return RequestResult.Ok(chatsInDb.Select(_chatConverter.ConvertToChatInApi).ToList());
        }

        /// <inheritdoc/>
        public async Task<RequestResult<Chat>> GetChatByIdAsync(Guid id)
        {
            using var chatStorage = _infoStorageFactory.CreateChatStorage();
            var chatInDb = await chatStorage.GetByIdAsync(id);
            return chatInDb is null ? 
                RequestResult.NotFound<Chat>($"Chat with identifier {id} not found") : 
                RequestResult.Ok(_chatConverter.ConvertToChatInApi(chatInDb));
        }

        /// <inheritdoc/>
        public async Task<RequestResult<List<Chat>>> GetByChatNameSubstringAsync(string chatNameSubstring)
        {
            using var chatStorage = _infoStorageFactory.CreateChatStorage();
            var chatsInDb = await chatStorage.GetByChatNameSubstringAsync(chatNameSubstring);

            return RequestResult.Ok(chatsInDb.Select(_chatConverter.ConvertToChatInApi).ToList());
        }
    }
}