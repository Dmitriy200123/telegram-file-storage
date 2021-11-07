using System;

namespace FileStorageAPI.Models
{
    public class Sender
    {
        public Guid? UserId { get; set; }
        public string? TelegramName { get; set; }
        public string? FullName { get; set; }
    }
}