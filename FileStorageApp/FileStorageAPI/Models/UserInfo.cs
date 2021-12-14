namespace FileStorageAPI.Models
{
    public class UserInfo
    {
        public string Name { get; set; }
        
        public string Avatar { get; set; }
        
        public bool HasTelegram { get; set; }

        public string Role { get; set; } = "null";
    }
}