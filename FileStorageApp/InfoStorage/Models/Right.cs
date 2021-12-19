using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileStorageApp.Data.InfoStorage.Models
{
    [Table("Rights")]
    public class Right : IModel
    {
        [Key]
        public Guid Id { get; set; }
        
        public Guid UserId { get; set; }

        public int Access { get; set; }

        [NotMapped]
        public Accesses AccessType
        {
            get => (Accesses) Access;

            set => Access = (int) value;
        }
        public virtual User User { get; set; }
    }
}