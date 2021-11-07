using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FileStorageAPI.Models;

namespace FileStorageAPI.Services
{
    /// <summary>
    /// Сервис для взаимодействия с информацией о чатах.
    /// </summary>
    public interface IChatService
    {
        /// <summary>
        /// Возвращает весь список чатов.
        /// </summary>
        Task<List<Chat>> GetAllChats();

        /// <summary>
        /// Возвращает чат по его идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор чата</param>
        Task<Chat> GetChatByIdAsync(Guid id);

        /// <summary>
        /// Возвращает список чатов по подстроке.
        /// </summary>
        /// <param name="chatNameSubstring">Подстрока названия чата</param>
        Task<List<Chat>> GetByChatNameSubstringAsync(string chatNameSubstring);
    }
}