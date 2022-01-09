using System.Collections.Generic;
using System.Linq;
using FileStorageApp.Data.InfoStorage.Models;

namespace FileStorageAPI.Extensions
{
    internal static class FileFilterExtension
    {
        public static List<File> FilterFiles(this IEnumerable<File> files, FileSender fileSender)
        {
            var chatsId = fileSender.Chats.Select(chat => chat.Id).ToHashSet();
            return files.Where(x => !x.ChatId.HasValue || chatsId.Contains(x.ChatId!.Value)).ToList();
        }
    }
}