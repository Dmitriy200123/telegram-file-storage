using Microsoft.AspNetCore.Identity;

namespace FileStorageAPI.Models
{
    /// <summary>
    /// Класс отвечающий за аутентифицированного пользователя
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Telegram Id, который может быть у пользователя
        /// </summary>
        public long? TelegramId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Имя пользователя</param>
        /// <param name="telegramId">Возможный telegram id</param>
        public ApplicationUser(string name, long? telegramId) : base(name)
        {
            TelegramId = telegramId;
        }

        /// <summary>
        /// Пустой конструктор необходимый для менеджера аутентификации
        /// </summary>
        public ApplicationUser()
        {
               
        }
    }
}