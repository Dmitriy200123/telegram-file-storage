using FileStorageAPI.Models;
using FileStorageApp.Data.InfoStorage.Models;

namespace FileStorageAPI.Converters
{
    public interface IUserConverter
    {
        UserInfo ConvertUser(User user);
    }
}