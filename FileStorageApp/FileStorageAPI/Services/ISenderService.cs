using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API;
using FileStorageAPI.Models;

namespace FileStorageAPI.Services
{
    /// <summary>
    /// Сервис для взаимодействия с отправителями файлов.
    /// </summary>
    public interface ISenderService
    {
        /// <summary>
        /// Получения отправителя по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор отправителя</param>
        Task<RequestResult<Sender>> GetSenderByIdAsync(Guid id);

        /// <summary>
        /// Получение всех отправителей.
        /// </summary>
        Task<RequestResult<List<Sender>>> GetSendersAsync();

        /// <summary>
        /// Получение отправителей по подстроке полного имени.
        /// </summary>
        /// <param name="fullName">подстрока полного имени</param>
        Task<RequestResult<List<Sender>>> GetSendersByUserNameSubstringAsync(string fullName);

        /// <summary>
        /// Получение отправителей по подстроке телеграм ника.
        /// </summary>
        /// <param name="telegramName">подстрока телеграм ника</param>
        Task<RequestResult<List<Sender>>> GetSendersByTelegramNameSubstringAsync(string telegramName);

        /// <summary>
        /// Добавление нового пользователя
        /// </summary>
        /// <param name="sender">Данные пользователя</param>
        Task<RequestResult<Sender>> AddSender(TelegramUser sender);
    }
}