using FileStorageApp.Data.InfoStorage.Config;
using FileStorageApp.Data.InfoStorage.Factories;
using FileStorageApp.Data.InfoStorage.Storages.Files;
using NUnit.Framework;

namespace InfoStorage.Tests
{
    public class FilesStorageShould
    {

        private IFilesStorage filesStorage;

        [SetUp]
        public void Setup()
        {
            var config = new DataBaseConfig();
            config.SetConnectionString("");
            var factory = new InfoStorageFactory(config);
            filesStorage = factory.CreateFileStorage();
        }
    }
}