using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FileStorageAPI.Converters;
using FileStorageAPI.Models;
using FileStorageApp.Data.InfoStorage.Factories;

namespace FileStorageAPI.Services
{
    /// <inheritdoc />
    public class SenderService : ISenderService
    {
        private readonly IInfoStorageFactory _infoStorageFactory;
        private readonly ISenderConverter _senderConverter;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="SenderService"/>
        /// </summary>
        /// <param name="infoStorageFactoryFactory">Фабрика для создания хранилища данных</param>
        /// <param name="senderConverter">Конвертер, для перобразования отправителей в API-контракты</param>
        public SenderService(IInfoStorageFactory infoStorageFactoryFactory, ISenderConverter senderConverter)
        {
            _infoStorageFactory = infoStorageFactoryFactory ?? throw new ArgumentNullException(nameof(infoStorageFactoryFactory));
            _senderConverter = senderConverter ?? throw new ArgumentNullException(nameof(senderConverter));
        }

        /// <inheritdoc />
        public async Task<RequestResult<Sender>> GetSenderByIdAsync(Guid id)
        {
            using var senderStorage = _infoStorageFactory.CreateFileSenderStorage();
            var fileSender = await senderStorage.GetByIdAsync(id);
            if (fileSender is not null)
                return RequestResult.Ok(_senderConverter.ConvertFileSender(fileSender));

            return RequestResult.NotFound<Sender>("User with identifier {id} not found");
        }

        /// <inheritdoc />
        public async Task<RequestResult<List<Sender>>> GetSendersAsync()
        {
            using var senderStorage = _infoStorageFactory.CreateFileSenderStorage();
            var fileSenders = await senderStorage.GetAllAsync();

            return RequestResult.Ok(_senderConverter.ConvertFileSenders(fileSenders));
        }

        /// <inheritdoc />
        public async Task<RequestResult<List<Sender>>> GetSendersByUserNameSubstringAsync(string fullName)
        {
            using var senderStorage = _infoStorageFactory.CreateFileSenderStorage();
            var fileSenders = await senderStorage.GetBySenderNameSubstringAsync(fullName);

            return RequestResult.Ok(_senderConverter.ConvertFileSenders(fileSenders));
        }

        /// <inheritdoc />
        public async Task<RequestResult<List<Sender>>> GetSendersByTelegramNameSubstringAsync(string telegramName)
        {
            using var senderStorage = _infoStorageFactory.CreateFileSenderStorage();
            var fileSenders = await senderStorage.GetByTelegramNameSubstringAsync(telegramName);

            return RequestResult.Ok(_senderConverter.ConvertFileSenders(fileSenders));
        }
    }
}