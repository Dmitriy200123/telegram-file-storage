using FileStorageApp.Data.InfoStorage.Storages.Chats;
using FileStorageApp.Data.InfoStorage.Storages.Files;
using FileStorageApp.Data.InfoStorage.Storages.FileSenders;

namespace FileStorageApp.Data.InfoStorage.Factories
{
    public interface IInfoStorageFactory
    {
        IChatStorage CreateChatStorage();

        IFilesStorage CreateFileStorage();

        IFileSenderStorage CreateFileSenderStorage();
    }
}