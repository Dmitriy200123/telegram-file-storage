using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FileStorageAPI.Models;

namespace FileStorageAPI.Services
{
    public interface ISenderService
    {
        Task<RequestResult<Sender>> GetSenderByIdAsync(Guid id);
        Task<RequestResult<List<Sender>>> GetSendersAsync();
        Task<RequestResult<List<Sender>>> GetSendersByUserNameSubstringAsync(string? fullName);
        Task<RequestResult<List<Sender>>> GetSendersByTelegramNameSubstringAsync(string? telegramName);

        Task<RequestResult<List<Sender>>> GetSendersByUserNameAndTelegramNameSubstringAsync(string? fullName,
            string? telegramName);
    }
}