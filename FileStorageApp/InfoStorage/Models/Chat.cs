using System;
using System.Collections.Generic;

namespace FileStorageApp.Data.InfoStorage.Models
{
    public class Chat : IModel
    {
        public string Name { get; set; }
        public Guid ImageId { get; set; }
        public Guid Id { get; set; }
        public List<File> Files { get; set; }
    }
}