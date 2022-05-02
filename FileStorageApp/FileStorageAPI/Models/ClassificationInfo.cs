using System;

namespace FileStorageAPI.Models
{
    /// <summary>
    /// Информация о классификации
    /// </summary>
    public class ClassificationInfo
    {
        /// <summary>
        /// Id классификации
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Имя классификации
        /// </summary>
        public string Name { get; set; }
    }
}