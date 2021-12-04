using System;
using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Factories;
using Newtonsoft.Json;

namespace DataBaseFiller.Actions
{
    public class ShowDataBaseInfoAction : IAction
    {
        public async Task DoActionAsync(IInfoStorageFactory infoStorageFactory)
        {
            using var filesStorage = infoStorageFactory.CreateFileStorage();
            using var chatStorage = infoStorageFactory.CreateChatStorage();
            using var senderStorage = infoStorageFactory.CreateFileSenderStorage();
            var files = await filesStorage.GetAllAsync();
            foreach (var file in files)
            {
                file.Chat = await chatStorage.GetByIdAsync(file.ChatId!.Value);
                file.FileSender = await senderStorage.GetByIdAsync(file.FileSenderId);
            }
            var line = await Task.Factory.StartNew(() => JsonConvert.SerializeObject(files, Formatting.Indented));
            Console.WriteLine(line);
        }
    }
}