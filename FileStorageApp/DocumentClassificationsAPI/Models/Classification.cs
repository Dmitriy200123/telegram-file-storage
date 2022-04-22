using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DocumentClassificationsAPI.Models
{
    /// <summary>
    /// Модель классификации
    /// </summary>
    public class Classification
    {
        /// <summary>
        /// Id классификации
        /// </summary>
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// Название классификации
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Список слов классификации
        /// </summary>
        public ICollection<ClassificationWord> ClassificationWords { get; set; } =
            new List<ClassificationWord>();
    }
}