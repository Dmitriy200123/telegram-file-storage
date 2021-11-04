using System;

namespace FileStorageAPI.Models
{
    public class Chat
    {
        public Chat()
        {
        }

        public Chat(Guid id, Guid imageId, string name)
        {
            Id = id;
            ImageId = imageId;
            Name = name;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid ImageId { get; set; }
    }
}