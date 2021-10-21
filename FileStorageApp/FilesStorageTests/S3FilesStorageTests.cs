using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Amazon.S3;
using FilesStorage;
using FluentAssertions;
using FilesStorage.Interfaces;

namespace FilesStorageTests
{
    public class S3FilesStorageShould
    {
        private const string serviceUrl = "http://localhost:4566";
        private IFilesStorage testClient;

        [SetUp]
        public void Setup()
        {
            var config = new AmazonS3Config();
            config.ServiceURL = serviceUrl;
            config.ForcePathStyle = true;

            testClient = new S3FilesStorageFactory(new S3FilesStorageOptions("123", "123",
                "test", config, S3CannedACL.PublicReadWrite, 2)).CreateAsync().GetAwaiter().GetResult();

            var files = testClient.GetFilesAsync().GetAwaiter().GetResult().S3Objects;

            foreach (var file in files)
            {
                testClient.DeleteFileAsync(file.Key);
            }
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public async Task SaveFile_HaveFile_WhenCalled()
        {
            using (FileStream fileStream = new FileStream(Path.GetTempFileName(),
                FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None,
                4096, FileOptions.RandomAccess | FileOptions.DeleteOnClose))
            {
                var key = fileStream.Name;
                fileStream.Write(new byte[8]);

                await testClient.SaveFileAsync(key, fileStream);

                var result = (await testClient.GetFilesAsync()).S3Objects;

                result.Should().HaveCount(1).And.Subject.First().Key.Should().Be(key);
            }
        }

        [Test]
        public async Task DeleteFileAsync_NoFile_AfterCalled()
        {
            using (FileStream fileStream = new FileStream(Path.GetTempFileName(),
                FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None,
                4096, FileOptions.RandomAccess | FileOptions.DeleteOnClose))
            {
                var key = fileStream.Name;
                fileStream.Write(new byte[8]);

                await testClient.SaveFileAsync(key, fileStream);

                testClient.DeleteFileAsync(key).GetAwaiter().GetResult();
                var result = (await testClient.GetFilesAsync()).S3Objects;

                result.Should().HaveCount(0);
            }
        }
    }
}