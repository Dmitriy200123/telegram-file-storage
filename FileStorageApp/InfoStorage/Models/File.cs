using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using FileStorageApp.Data.InfoStorage.Enums;

namespace FileStorageApp.Data.InfoStorage.Models
{
    [Table("Files")]
    public class File : IModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        [MaxLength(255)]
        public string Extension { get; set; }

        [Required]
        public int TypeId { get; set; }

        [NotMapped]
        public FileType Type
        {
            get => (FileType) TypeId;

            set => TypeId = (int) value;
        }

        [Required]
        public DateTime UploadDate { get; set; }

        [Required]
        public Guid FileSenderId { get; set; }
        
        public Guid? ChatId { get; set; }

        [ForeignKey("FileSenderId")]
        public virtual FileSender FileSender { get; set; }

        [ForeignKey("ChatId")]
        public virtual Chat? Chat { get; set; }//если сделать не nullable - при создании таблички не дает создать nullable столбец
    }
}