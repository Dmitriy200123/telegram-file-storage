using System;
using System.ComponentModel.DataAnnotations;

namespace DocumentClassificationsAPI.Models
{
    public class ClassificationWord
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Value { get; set; }
        
        public Guid ClassificationId { get; set; }
    }
}