using System;

namespace FileStorageAPI.Models
{
    /// <summary>
    /// Класс для хранения идентификатора пользователя и его фамилии и имени.
    /// </summary>
    public class UserIdAndFio
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Фамилия и имя.
        /// </summary>
        public string Name { get; set; }
    }
}