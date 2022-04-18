using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DocumentClassificationsAPI.Models
{
    public class ClassificationInsert
    {
        [Required]
        public string Name { get; set; }

        public ICollection<ClassificationWordInsert> ClassificationWords { get; set; } =
            new List<ClassificationWordInsert>();
    }
}