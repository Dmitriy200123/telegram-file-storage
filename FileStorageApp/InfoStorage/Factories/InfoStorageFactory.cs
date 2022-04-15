using FileStorageApp.Data.InfoStorage.Config;
using FileStorageApp.Data.InfoStorage.Storages.Chats;
using FileStorageApp.Data.InfoStorage.Storages.Files;
using FileStorageApp.Data.InfoStorage.Storages.FileSenders;
using FileStorageApp.Data.InfoStorage.Storages.MarkedTextsTags;
using FileStorageApp.Data.InfoStorage.Storages.Rights;
using FileStorageApp.Data.InfoStorage.Storages.Users;

namespace FileStorageApp.Data.InfoStorage.Factories
{
    public class InfoStorageFactory : IInfoStorageFactory
    {
        private readonly IDataBaseConfig _dataBaseConfig;

        public InfoStorageFactory(IDataBaseConfig dataBaseConfig)
        {
            _dataBaseConfig = dataBaseConfig;
        }

        public IChatStorage CreateChatStorage() => new ChatStorage(_dataBaseConfig);

        public IFilesStorage CreateFileStorage() => new FilesStorage(_dataBaseConfig);

        public IFileSenderStorage CreateFileSenderStorage() => new FileSenderStorage(_dataBaseConfig);

        public IUsersStorage CreateUsersStorage() => new UsersStorage(_dataBaseConfig);

        public IMarkedTextTagsStorage CreateMarkedTextTagsStorage() => new MarkedTextTagsStorage(_dataBaseConfig);

        public IRightsStorage CreateRightsStorage() => new RightsStorage(_dataBaseConfig);
    }
}