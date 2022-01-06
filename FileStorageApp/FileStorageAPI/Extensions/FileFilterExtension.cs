using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Factories;
using FileStorageApp.Data.InfoStorage.Models;

namespace FileStorageAPI.Extensions
{
    public static class FileFilterExtension
    {
        public static async Task<List<File>> FilterFiles(this List<File> files, Guid fileSenderId, IInfoStorageFactory infoStorageFactory)
        {
            using var sendersStorage = infoStorageFactory.CreateFileSenderStorage();
            var fileSender = await sendersStorage.GetByIdAsync(fileSenderId);
            var chatsId = fileSender.Chats.Select(x => x.Id).ToList();
            chatsId.Add(Guid.Empty);
            return files.Where(x => chatsId.Contains(x.ChatId!.Value)).ToList();
        }
    }
}