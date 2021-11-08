using System;
using System.ComponentModel.DataAnnotations;

namespace FileStorageAPI.Models
{
    /// <summary>
    /// Информация об отправителе из Telegram.
    /// </summary>
    public class Sender
    {
        /// <summary>
        /// Идентификатор отправителя.
        /// </summary>
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        /// Ник отправителя в телеграме.
        /// </summary>
        [Required]
        public string TelegramName { get; set; }

        /// <summary>
        /// Полное имя отправителя.
        /// </summary>
        [Required]
        public string FullName { get; set; }
    }
}