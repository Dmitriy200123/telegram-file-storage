using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileStorageApp.Data.InfoStorage.Models
{
    [Table("Chats")]
    public class Chat : IModel
    {
        [Key] public Guid Id { get; set; }
        [Required] [MaxLength(255)] public string Name { get; set; }
        [Required] public Guid ImageId { get; set; }
        public virtual ICollection<File> Files { get; set; }
    }
}