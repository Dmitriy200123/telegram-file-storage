using FileStorageApp.Data.InfoStorage.Config;
using FileStorageApp.Data.InfoStorage.Factories;
using FileStorageApp.Data.InfoStorage.Storages.FileSenders;
using NUnit.Framework;

namespace InfoStorage.Tests
{
    public class FileSenderStorageShould
    {
        private IFileSenderStorage fileSenderStorage;

        [SetUp]
        public void Setup()
        {
            var config = new DataBaseConfig();
            config.SetConnectionString("");
            var factory = new InfoStorageFactory(config);
            fileSenderStorage = factory.CreateFileSenderStorage();
        }
    }
}