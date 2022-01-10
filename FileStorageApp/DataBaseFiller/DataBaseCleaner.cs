using System.Threading.Tasks;
using FilesStorage.Interfaces;
using FileStorageApp.Data.InfoStorage.Factories;

namespace DataBaseFiller
{
    public static class DataBaseCleaner
    {
        public static async Task CleanUpAsync(IInfoStorageFactory infoStorageFactory, IFilesStorageFactory filesStorageFactory)
        {
            using var filesStorage = infoStorageFactory.CreateFileStorage();
            using var chatStorage = infoStorageFactory.CreateChatStorage();
            using var senderStorage = infoStorageFactory.CreateFileSenderStorage();
            foreach (var element in await senderStorage.GetAllAsync())
                await senderStorage.DeleteAsync(element.Id);
            foreach (var element in await chatStorage.GetAllAsync())
                await chatStorage.DeleteAsync(element.Id);
            foreach (var element in await filesStorage.GetAllAsync())
                await filesStorage.DeleteAsync(element.Id);

            using var physicalFilesStorage = await filesStorageFactory.CreateAsync();
            foreach (var file in await physicalFilesStorage.GetFilesAsync())
                await physicalFilesStorage.DeleteFileAsync(file.Key);
        }
    }
}