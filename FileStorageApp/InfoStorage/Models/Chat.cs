using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileStorageApp.Data.InfoStorage.Models
{
    public class Chat : IModel
    {
        public string Name { get; set; }
        public Guid ImageId { get; set; }
        public Guid Id { get; set; }
        public virtual ICollection<File> Files { get; set; }
    }
}