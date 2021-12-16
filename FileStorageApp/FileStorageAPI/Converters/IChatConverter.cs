using FileStorageAPI.Models;
using ChatInDb = FileStorageApp.Data.InfoStorage.Models.Chat;

namespace FileStorageAPI.Converters
{
    /// <summary>
    /// Конвертор для преобразования чатов в API-контракты.
    /// </summary>
    public interface IChatConverter
    {
        /// <summary>
        /// Конвертирует информацию о чате в API-контракты.
        /// </summary>
        /// <param name="chat">Информация о чате из базы данных</param>
        Chat ConvertToChatInApi(ChatInDb? chat);
    }
}