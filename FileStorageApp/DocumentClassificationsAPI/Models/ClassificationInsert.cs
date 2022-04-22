using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DocumentClassificationsAPI.Models
{
    /// <summary>
    /// Модель добавляемой классификации
    /// </summary>
    public class ClassificationInsert
    {
        /// <summary>
        /// Название классификации
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Список слов классификации
        /// </summary>
        public ICollection<ClassificationWordInsert> ClassificationWords { get; set; } =
            new List<ClassificationWordInsert>();
    }
}