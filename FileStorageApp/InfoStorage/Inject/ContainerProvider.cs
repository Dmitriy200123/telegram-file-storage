using FileStorageApp.Data.InfoStorage.Config;
using FileStorageApp.Data.InfoStorage.Tables.Chat;
using FileStorageApp.Data.InfoStorage.Tables.Files;
using FileStorageApp.Data.InfoStorage.Tables.FileSender;
using Microsoft.Extensions.DependencyInjection;

namespace FileStorageApp.Data.InfoStorage.Inject
{
    internal static class ContainerProvider
    {
        public static ServiceProvider CreateContainer()
        {
            var container = new ServiceCollection();
            container.AddSingleton<IDataBaseConfig, DataBaseConfig>();
            container.AddSingleton<IMainStorage, MainStorage>();
            container.AddSingleton<IChatTable, ChatTable>();
            container.AddSingleton<IFilesTable, FilesTable>();
            container.AddSingleton<IFileSenderTable, FileSenderTable>();
            return container.BuildServiceProvider();
        }
    }
}