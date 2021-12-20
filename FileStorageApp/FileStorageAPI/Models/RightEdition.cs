using System;
using System.ComponentModel.DataAnnotations;

namespace FileStorageAPI.Models
{
    /// <summary>
    /// Класс отвечающий за изменение прав пользователя
    /// </summary>
    public class RightEdition
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        [Required]
        public Guid UserId { get; set; }
        /// <summary>
        /// Права, которые нужно выдать
        /// </summary>
        public int[]? Grant { get; set; }
        /// <summary>
        /// Права которые нужно забрать
        /// </summary>
        public int[]? Revoke { get; set; }
    }
}