using System.ComponentModel.DataAnnotations;

namespace FileStorageAPI.Models
{
    /// <summary>
    /// Пользователя телеграмма
    /// </summary>
    public class TelegramUser
    {
        /// <summary>
        /// Ник отправителя в телеграме.
        /// </summary>
        [Required]
        public string TelegramUserName { get; set; }

        /// <summary>
        /// Полное имя отправителя.
        /// </summary>
        [Required]
        public string FullName { get; set; }
        
        /// <summary>
        /// Уникальный идентификатор пользователя в telegram.
        /// </summary>
        [Required]
        public long TelegramId { get; set; }
    }
}