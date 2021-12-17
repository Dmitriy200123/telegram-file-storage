using System;

namespace FileStorageAPI.Models
{
    public class RightEdition
    {
        public Guid? UserId { get; set; }
        public int[]? Grant { get; set; }
        public int[]? Revoke { get; set; }
    }
}