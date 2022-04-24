using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileStorageApp.Data.InfoStorage.Models
{
    [Table("DocumentClassificationWords")]
    internal class DocumentClassificationWord : IModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Value { get; set; }
        
        public Guid ClassificationId { get; set; }
        
        [ForeignKey("ClassificationId")]
        public virtual DocumentClassification Classification { get; set; }
    }
}