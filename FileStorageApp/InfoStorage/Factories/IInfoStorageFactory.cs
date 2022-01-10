using FileStorageApp.Data.InfoStorage.Storages.AccessStorage;
using FileStorageApp.Data.InfoStorage.Storages.Chats;
using FileStorageApp.Data.InfoStorage.Storages.Files;
using FileStorageApp.Data.InfoStorage.Storages.FileSenders;
using FileStorageApp.Data.InfoStorage.Storages.MarkedTextsTags;
using FileStorageApp.Data.InfoStorage.Storages.Users;

namespace FileStorageApp.Data.InfoStorage.Factories
{
    public interface IInfoStorageFactory
    {
        IChatStorage CreateChatStorage();

        IFilesStorage CreateFileStorage();

        IFileSenderStorage CreateFileSenderStorage();

        IUsersStorage CreateUsersStorage();

        IRightsStorage CreateRightsStorage();

        IMarkedTextTagsStorage CreateMarkedTextTagsStorage();
    }
}