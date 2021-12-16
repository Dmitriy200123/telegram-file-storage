using FileStorageAPI.Models;
using FileStorageApp.Data.InfoStorage.Models;

namespace FileStorageAPI.Converters
{
    /// <summary>
    /// Конвертация пользователя из базы данных
    /// </summary>
    public interface IUserConverter
    {
        /// <summary>
        /// Конвертирует пользователя из базы в апи контракт
        /// </summary>
        /// <param name="user">Пользователь из базы данных</param>
        /// <returns></returns>
        UserInfo ConvertUser(User user);
    }
}