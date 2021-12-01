using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Factories;

namespace DataBaseFiller
{
    public static class DataBaseCleaner
    {
        public static async Task CleanUp(IInfoStorageFactory infoStorageFactory)
        {
            using var filesStorage = infoStorageFactory.CreateFileStorage();
            using var chatStorage = infoStorageFactory.CreateChatStorage();
            using var senderStorage = infoStorageFactory.CreateFileSenderStorage();
            foreach (var element in senderStorage.GetAllAsync().Result)
                await senderStorage.DeleteAsync(element.Id);
            foreach (var element in chatStorage.GetAllAsync().Result)
                await chatStorage.DeleteAsync(element.Id);
            foreach (var element in filesStorage.GetAllAsync().Result)
                await filesStorage.DeleteAsync(element.Id);
        }
    }
}