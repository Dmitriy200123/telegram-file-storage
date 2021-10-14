using FileStorageApp.Data.InfoStorage.Config;
using FileStorageApp.Data.InfoStorage.Inject;
using FileStorageApp.Data.InfoStorage.Tables.Chat;
using FileStorageApp.Data.InfoStorage.Tables.Files;
using FileStorageApp.Data.InfoStorage.Tables.FileSender;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace FileStorageApp.Data.InfoStorage
{
    public class Storage
    {
        public IChatTable ChatTable => serviceProvider.GetService<IChatTable>();
        public IFileSenderTable FileSenderTable => serviceProvider.GetService<IFileSenderTable>();
        public IFilesTable FilesTable => serviceProvider.GetService<IFilesTable>();
        private readonly ServiceProvider serviceProvider;

        public Storage(string connectionString)
        {
            serviceProvider = ContainerProvider.CreateContainer();
            var config = serviceProvider.GetService<IDataBaseConfig>();
            config?.SetConnectionString(connectionString);
        }
    }
}