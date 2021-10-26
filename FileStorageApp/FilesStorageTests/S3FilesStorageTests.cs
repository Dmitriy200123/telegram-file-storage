using System;
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
        private IFilesStorage _sut;

        [SetUp]
        public void Setup()
        {
            var config = new AmazonS3Config();
            config.ServiceURL = serviceUrl;
            config.ForcePathStyle = true;

            _sut = new S3FilesStorageFactory(new S3FilesStorageOptions("123", "123",
                "test", config, S3CannedACL.PublicReadWrite, 2)).CreateAsync().GetAwaiter().GetResult();
        }

        [TearDown]
        public void TearDown()
        {
            var files = _sut.GetFilesAsync().GetAwaiter().GetResult();

            foreach (var file in files)
            {
                _sut.DeleteFileAsync(file.Key);
            }
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

                await _sut.SaveFileAsync(key, fileStream);

                var result = await _sut.GetFilesAsync();

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

                await _sut.SaveFileAsync(key, fileStream);

                await _sut.DeleteFileAsync(key);
                var result = await _sut.GetFilesAsync();

                result.Should().HaveCount(0);
            }
        }

        [Test]
        public async Task GetFileAsync_ThrowNorFoundExc_ThenCalledToUnknownFile()
        {
            var key = "smth";
            Func<Task> act = _sut.Awaiting(x => x.GetFileAsync(key));
            await act.Should().ThrowAsync<FileNotFoundException>()
                .WithMessage("Not found file with key=smth in bucket=test");
        }
    }
}