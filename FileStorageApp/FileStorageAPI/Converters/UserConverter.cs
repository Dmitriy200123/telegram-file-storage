using FileStorageAPI.Models;
using FileStorageApp.Data.InfoStorage.Models;

namespace FileStorageAPI.Converters
{
    class UserConverter : IUserConverter
    {
        public UserInfo ConvertUser(User user)
        {
            return new UserInfo
            {
                Name = user.Name,
                Avatar = user.Avatar,
                HasTelegram = user.TelegramId is not null,
            };
        }

        public UserIdAndFio ConvertUserToIdAndFio(User user)
        {
            return new UserIdAndFio
            {
                Id = user.Id,
                Name = user.Name
            };
        }
    }
}