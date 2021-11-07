using System;

namespace FileStorageAPI.Models
{
    /// <summary>
    /// Информация об отправителе из Telegram.
    /// </summary>
    public class Sender
    {
        /// <summary>
        /// Идентификатор отправителя
        /// </summary>
        public Guid? UserId { get; set; }
        /// <summary>
        /// Ник отправителя в телеграме
        /// </summary>
        public string? TelegramName { get; set; }
        /// <summary>
        /// Полное имя отправителя
        /// </summary>
        public string? FullName { get; set; }
    }
}