using System.Threading.Tasks;
using FilesStorage.Interfaces;
using FileStorageApp.Data.InfoStorage.Factories;

namespace DataBaseFiller
{
    public static class DataBaseCleaner
    {
        public static async Task CleanUp(IInfoStorageFactory infoStorageFactory, IFilesStorageFactory filesStorageFactory)
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

            using var physicalFilesStorage = await filesStorageFactory.CreateAsync();
            foreach (var file in await physicalFilesStorage.GetFilesAsync())
                await physicalFilesStorage.DeleteFileAsync(file.Key);
        }
    }
}