using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileStorageApp.Data.InfoStorage.Models
{
    [Table("MarkedTextTags")]
    public class MarkedTextTags : IModel
    {
        [Key]
        public Guid Id { get; set; }

        public string TitleTag { get; set; }

        public string DescriptionTag { get; set; }
    }
}