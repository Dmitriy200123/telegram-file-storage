using System;

namespace FileStorageAPI.Models
{
    public class Chat
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid ImageId { get; set; }
    }
}