using System;
using FileStorageApp.Data.InfoStorage.Factories;
using Newtonsoft.Json;

namespace DataBaseFiller.Actions
{
    public class ShowDataBaseInfoAction : IAction
    {
        public void DoAction(IInfoStorageFactory infoStorageFactory)
        {
            using var filesStorage = infoStorageFactory.CreateFileStorage();
            using var chatStorage = infoStorageFactory.CreateChatStorage();
            using var senderStorage = infoStorageFactory.CreateFileSenderStorage();
            var files = filesStorage.GetAllAsync().Result;
            foreach (var file in files)
            {
                file.Chat = chatStorage.GetByIdAsync(file.ChatId!.Value).Result;
                file.FileSender = senderStorage.GetByIdAsync(file.FileSenderId).Result;
            }
            var line = JsonConvert.SerializeObject(files, Formatting.Indented);
            Console.WriteLine(line);
        }
    }
}