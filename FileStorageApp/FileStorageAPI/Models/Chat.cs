using System;

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
        public Guid Id { get; set; }

        /// <summary>
        /// Название чата.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Идентификатор изображения чата.
        /// </summary>
        public Guid ImageId { get; set; }
    }
}