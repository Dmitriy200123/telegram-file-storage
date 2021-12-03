using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileStorageApp.Data.InfoStorage.Models
{
    [Table("Users")]
    public class User : IModel
    {
        [Key]
        public Guid Id { get; set; }
        
        public long? TelegramId { get; set; }

        [ForeignKey("TelegramId")]
        public virtual FileSender FileSenders { get; set; }
        
    }
}