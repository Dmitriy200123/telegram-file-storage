using System.ComponentModel.DataAnnotations;

namespace DocumentClassificationsAPI.Models
{
    /// <summary>
    /// Модель добавляемого слова
    /// </summary>
    public class ClassificationWordInsert
    {
        /// <summary>
        /// Слово
        /// </summary>
        [Required]
        public string Value { get; set; }
    }
}