using System;
using System.ComponentModel.DataAnnotations;

namespace FileStorageAPI.Models
{
    /// <summary>
    /// Информация о чате из Telegram.
    /// </summary>
    public class Chat
    {
        /// <summary>
        /// Идентификатор чата.
        /// </summary>
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// Название чата.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Идентификатор изображения чата.
        /// </summary>
        public Guid ImageId { get; set; }
    }
}