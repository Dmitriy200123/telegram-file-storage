using System.Threading.Tasks;
using NUnit.Framework;
using Amazon.S3;
using FilesStorage;
using FilesStorage.Interfaces;

namespace FilesStorageTests
{
    public class Tests
    {
        private const string serviceUrl = "http://localhost:4566";
        private IFilesStorage testClient;

        [SetUp]
        public async void Setup()
        {
            var config = new AmazonS3Config();
            config.ServiceURL = serviceUrl;
            config.ForcePathStyle = true;
            testClient =
                await new S3FilesStorageFactory(new S3FilesStorageOptions("123", "123",
                    "test", config, S3CannedACL.PublicReadWrite, 2)).CreateAsync();
        }

        [TearDown]
        public void TearDown()
        {
        }
    }
}