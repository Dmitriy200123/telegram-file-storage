using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FileStorageAPI.Converters;
using FileStorageAPI.Models;
using FileStorageApp.Data.InfoStorage.Factories;
using FileStorageApp.Data.InfoStorage.Storages.FileSenders;

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

        public async Task<RequestResult<Sender>> GetSenderByIdAsync(Guid id)
        {
            var fileSender = await fileSenderStorage.GetByIdAsync(id);
            if (fileSender != null)
                return RequestResult.Ok(senderConverter.ConvertFileSender(fileSender));

            return RequestResult.NotFound<Sender>("User with identifier {id} not found");
        }

        public async Task<RequestResult<List<Sender>>> GetSendersAsync()
        {
            var fileSenders = await fileSenderStorage.GetAllAsync();
            return RequestResult.Ok(senderConverter.ConvertFileSenders(fileSenders));
        }

        public async Task<RequestResult<List<Sender>>> GetSendersByUserNameSubstringAsync(string? fullName)
        {
            if (fullName == null)
                return RequestResult.NotFound<List<Sender>>("fullName is empty");

            var fileSenders = await fileSenderStorage.GetBySenderNameSubstringAsync(fullName);
            return RequestResult.Ok(senderConverter.ConvertFileSenders(fileSenders));
        }

        public async Task<RequestResult<List<Sender>>> GetSendersByTelegramNameSubstringAsync(string? telegramName)
        {
            if (telegramName == null)
                return RequestResult.NotFound<List<Sender>>("telegramName is empty");

            var fileSenders = await fileSenderStorage.GetByTelegramNameSubstringAsync(telegramName);
            return RequestResult.Ok(senderConverter.ConvertFileSenders(fileSenders));
        }

        public async Task<RequestResult<List<Sender>>> GetSendersByUserNameAndTelegramNameSubstringAsync(
            string? fullName, string? telegramName)
        {
            var sendersByTelegramName = await GetSendersByUserNameSubstringAsync(telegramName);
            var sendersByFullName = await GetSendersByTelegramNameSubstringAsync(fullName);
            if (sendersByFullName.ResponseCode == HttpStatusCode.OK && sendersByTelegramName.ResponseCode == HttpStatusCode.OK)
                return RequestResult.Ok(sendersByFullName.Value!.Intersect(sendersByTelegramName.Value!).ToList());
            if (sendersByFullName.ResponseCode == HttpStatusCode.OK)
                return sendersByFullName;
            if (sendersByTelegramName.ResponseCode == HttpStatusCode.OK)
                return sendersByTelegramName;
            return RequestResult.NotFound<List<Sender>>($"{sendersByFullName.Message} and {sendersByTelegramName.Message}");
        }
    }
}