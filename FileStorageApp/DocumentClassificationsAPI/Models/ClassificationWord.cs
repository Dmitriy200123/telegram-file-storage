using System;
using System.ComponentModel.DataAnnotations;

namespace DocumentClassificationsAPI.Models
{
    /// <summary>
    /// Модель слова классификации
    /// </summary>
    public class ClassificationWord
    {
        /// <summary>
        /// Id слова
        /// </summary>
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// Слово
        /// </summary>
        [Required]
        public string Value { get; set; }
        
        /// <summary>
        /// Id принадлежащей классификации
        /// </summary>
        public Guid ClassificationId { get; set; }
    }
}