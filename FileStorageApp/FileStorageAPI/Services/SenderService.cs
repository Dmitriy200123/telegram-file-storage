using System;
using System.Linq;
using System.Threading.Tasks;
using FileStorageAPI.Converters;
using FileStorageAPI.Models;
using FileStorageApp.Data.InfoStorage.Factories;
using FileStorageApp.Data.InfoStorage.Models;
using FileStorageApp.Data.InfoStorage.Storages.FileSenders;
using Microsoft.AspNetCore.Mvc;

namespace FileStorageAPI.Services
{
    public class SenderService : ISenderService
    {
        private readonly ISenderConverter senderConverter;
        private readonly IFileSenderStorage fileSenderStorage;

        public SenderService(IInfoStorageFactory infoStorageFactory, ISenderConverter senderConverter)
        {
            this.senderConverter = senderConverter;
            fileSenderStorage = infoStorageFactory.CreateFileSenderStorage();
        }

        public async Task<Sender> GetSenderById(Guid id)
        {
            var fileSender = await fileSenderStorage.GetByIdAsync(id);
            return fileSender is null ? null : senderConverter.ConvertFileSender(fileSender);
        }

        public async Task<Sender> CreateSender([FromBody] Sender sender)
        {
            var fileSender = new FileSender
            {
                TelegramUserName = sender.TelegramName,
                FullName = sender.FullName,
            };
            await fileSenderStorage.AddAsync(fileSender);
            var result = await fileSenderStorage.GetByTelegramNameSubstringAsync(sender.TelegramName);
            return senderConverter.ConvertFileSender(result.First(x => x.FullName == sender.FullName));
        }

        public async Task<Sender> UpdateSender(Guid id, [FromBody] Sender sender)
        {
            var fileSender = await fileSenderStorage.GetByIdAsync(id);
            if (sender.FullName != null) fileSender.FullName = sender.FullName;
            if (sender.TelegramName != null) fileSender.TelegramUserName = sender.TelegramName;
            await fileSenderStorage.UpdateAsync(fileSender);
            return senderConverter.ConvertFileSender(fileSender);
        }
    }
}