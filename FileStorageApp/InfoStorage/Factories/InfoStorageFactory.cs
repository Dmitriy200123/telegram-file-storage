using FileStorageApp.Data.InfoStorage.Config;
using FileStorageApp.Data.InfoStorage.Storages.Chats;
using FileStorageApp.Data.InfoStorage.Storages.Files;
using FileStorageApp.Data.InfoStorage.Storages.FileSenders;

namespace FileStorageApp.Data.InfoStorage.Factories
{
    internal record InfoStorageFactory(IDataBaseConfig DataBaseConfig) : IInfoStorageFactory
    {
        private IDataBaseConfig DataBaseConfig { get; } = DataBaseConfig;

        public IChatStorage CreateChatStorage() => new ChatStorage(DataBaseConfig);

        public IFilesStorage CreateFileStorage() => new FilesStorage(DataBaseConfig);

        public IFileSenderStorage CreateFileSenderStorage() => new FileSenderStorage(DataBaseConfig);
    }
}