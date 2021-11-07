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
    /// <inheritdoc />
    public class SenderService : ISenderService
    {
        private readonly ISenderConverter _senderConverter;
        private readonly IFileSenderStorage _fileSenderStorage;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoStorageFactory"></param>
        /// <param name="senderConverter"></param>
        public SenderService(IInfoStorageFactory infoStorageFactory, ISenderConverter senderConverter)
        {
            _senderConverter = senderConverter;
            _fileSenderStorage = infoStorageFactory.CreateFileSenderStorage();
        }
        /// <inheritdoc />
        public async Task<RequestResult<Sender>> GetSenderByIdAsync(Guid id)
        {
            var fileSender = await _fileSenderStorage.GetByIdAsync(id);
            if (fileSender != null)
                return RequestResult.Ok(_senderConverter.ConvertFileSender(fileSender));

            return RequestResult.NotFound<Sender>("User with identifier {id} not found");
        }
        /// <inheritdoc />
        public async Task<RequestResult<List<Sender>>> GetSendersAsync()
        {
            var fileSenders = await _fileSenderStorage.GetAllAsync();
            return RequestResult.Ok(_senderConverter.ConvertFileSenders(fileSenders));
        }
        /// <inheritdoc />
        public async Task<RequestResult<List<Sender>>> GetSendersByUserNameSubstringAsync(string? fullName)
        {
            if (fullName == null)
                return RequestResult.NotFound<List<Sender>>("fullName is empty");

            var fileSenders = await _fileSenderStorage.GetBySenderNameSubstringAsync(fullName);
            return RequestResult.Ok(_senderConverter.ConvertFileSenders(fileSenders));
        }
        /// <inheritdoc />
        public async Task<RequestResult<List<Sender>>> GetSendersByTelegramNameSubstringAsync(string? telegramName)
        {
            if (telegramName == null)
                return RequestResult.NotFound<List<Sender>>("telegramName is empty");

            var fileSenders = await _fileSenderStorage.GetByTelegramNameSubstringAsync(telegramName);
            return RequestResult.Ok(_senderConverter.ConvertFileSenders(fileSenders));
        }
        /// <inheritdoc />
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