using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileStorageApp.Data.InfoStorage.Models
{
    [Table("DocumentClassifications")]
    public class DocumentClassification : IModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        public virtual ICollection<DocumentClassificationWord> ClassificationWords { get; set; } =
            new List<DocumentClassificationWord>();

        public virtual ICollection<File> Documents { get; set; } = new List<File>();
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}