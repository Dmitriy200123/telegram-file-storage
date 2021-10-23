using FileStorageApp.Data.InfoStorage.Config;
using FileStorageApp.Data.InfoStorage.Storages.Chats;
using FileStorageApp.Data.InfoStorage.Storages.Files;
using FileStorageApp.Data.InfoStorage.Storages.FileSenders;

namespace FileStorageApp.Data.InfoStorage.Factories
{
    public class InfoStorageFactory : IInfoStorageFactory
    {
        private readonly IDataBaseConfig _dataBaseConfig;

        public InfoStorageFactory(IDataBaseConfig dataBaseConfig)
        {
            _dataBaseConfig = dataBaseConfig;
        }

        public IChatStorage CreateChatStorage()
        {
            return new ChatStorage(_dataBaseConfig);
        }

        public IFilesStorage CreateFileStorage()
        {
            return new FilesStorage(_dataBaseConfig);
        }

        public IFileSenderStorage CreateFileSenderStorage()
        {
            return new FileSenderStorage(_dataBaseConfig);
        }
    }
}