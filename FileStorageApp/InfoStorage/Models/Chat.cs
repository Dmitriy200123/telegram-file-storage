using System;

namespace FileStorageApp.Data.InfoStorage.Models
{
    public class Chat
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid ImageId { get; set; }
    }
}