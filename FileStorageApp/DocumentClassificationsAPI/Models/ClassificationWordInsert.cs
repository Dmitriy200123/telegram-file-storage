using System.ComponentModel.DataAnnotations;

namespace DocumentClassificationsAPI.Models
{
    public class ClassificationWordInsert
    {
        [Required]
        public string Value { get; set; }
    }
}