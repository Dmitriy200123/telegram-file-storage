using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileStorageApp.Data.InfoStorage.Models
{
    [Table("Files")]
    public class File : IModel
    {
        [Required] [MaxLength(255)] public string Name { get; set; }
        [Required] [MaxLength(255)] public string Extension { get; set; }
        [Required] [MaxLength(255)] public string Type { get; set; }
        [Required] public DateTime UploadDate { get; set; }
        [Required] public Guid FileSenderId { get; set; }
        [Required] public Guid ChatId { get; set; }
        [Key] public Guid Id { get; set; }
        [ForeignKey("FileSenderId")] public virtual FileSender FileSender { get; set; }
        [ForeignKey("ChatId")] public virtual Chat Chat { get; set; }
    }
}