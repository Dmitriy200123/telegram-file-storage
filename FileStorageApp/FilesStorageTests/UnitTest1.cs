using System.Threading.Tasks;
using NUnit.Framework;
using Amazon.S3;
using FilesStorage;

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
                await new S3FilesStorageFactory().CreateAsync(new S3FilesStorageOptions("123", "123",
                    "test", config, S3CannedACL.PublicReadWrite));
        }

        [TearDown]
        public void TearDown()
        {
        }
    }
}