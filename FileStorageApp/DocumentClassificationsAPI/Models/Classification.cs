using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DocumentClassificationsAPI.Models
{
    public class Classification
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<ClassificationWord> ClassificationWords { get; set; } =
            new List<ClassificationWord>();
    }
}