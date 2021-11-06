using FileStorageAPI.Models;
using ChatInDb = FileStorageApp.Data.InfoStorage.Models.Chat;

namespace FileStorageAPI.Converters
{
    public interface IChatConverter
    {
        Chat ConvertToChatInApi(ChatInDb chat);
    }
}