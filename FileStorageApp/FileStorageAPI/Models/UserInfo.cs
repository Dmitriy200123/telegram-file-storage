namespace FileStorageAPI.Models
{
    /// <summary>
    /// Информация о пользователе
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Ссылка на аватар пользователя на GitLab
        /// </summary>
        public string Avatar { get; set; }
        
        /// <summary>
        /// Привязан ли телеграм у пользователя
        /// </summary>
        public bool HasTelegram { get; set; }
    }
}