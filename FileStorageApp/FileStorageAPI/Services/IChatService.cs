using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FileStorageAPI.Models;

namespace FileStorageAPI.Services
{
    public interface IChatService
    {
        Task<Chat> GetChatByIdAsync(Guid id);

        Task<List<Chat>> GetByChatNameSubstringAsync(string chatName);
    }
}