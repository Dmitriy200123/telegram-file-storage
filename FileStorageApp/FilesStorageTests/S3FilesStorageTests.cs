using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Amazon.S3;
using FilesStorage;
using FluentAssertions;
using FilesStorage.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FilesStorageTests
{
    public class S3FilesStorageShould
    {
        private IFilesStorageFactory _sutFactory;

        private static readonly IConfigurationRoot Config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.test.json")
            .Build();

        [SetUp]
        public void Setup()
        {
            var config = new AmazonS3Config {ServiceURL = Config["ServiceUrl"], ForcePathStyle = true};

            _sutFactory = new S3FilesStorageFactory(new S3FilesStorageOptions(Config["AccessKey"]
                , Config["SecretKey"],
                Config["BucketName"], config, S3CannedACL.PublicReadWrite,
                TimeSpan.FromHours(1)));
        }

        [TearDown]
        public async Task TearDown()
        {
            using var sut = await _sutFactory.CreateAsync();

            var files = await sut.GetFilesAsync();

            foreach (var file in files)
                await sut.DeleteFileAsync(file.Key);
        }

        [Test]
        public async Task SaveFile_HaveFile_WhenCalled()
        {
            await using var fileStream = new FileStream(Path.GetTempFileName(),
                FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None,
                4096, FileOptions.RandomAccess | FileOptions.DeleteOnClose);
            using var sut = await _sutFactory.CreateAsync();
            var key = fileStream.Name;
            fileStream.Write(new byte[8]);

            await sut.SaveFileAsync(key, fileStream);

            var result = await sut.GetFilesAsync();

            result.Should().HaveCount(1).And.Subject.First().Key.Should().Be(key);
        }

        [Test]
        public async Task DeleteFileAsync_NoFile_AfterCalled()
        {
            await using var fileStream = new FileStream(Path.GetTempFileName(),
                FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None,
                4096, FileOptions.RandomAccess | FileOptions.DeleteOnClose);
            using var sut = await _sutFactory.CreateAsync();
            var key = fileStream.Name;
            fileStream.Write(new byte[8]);

            await sut.SaveFileAsync(key, fileStream);

            await sut.DeleteFileAsync(key);
            var result = await sut.GetFilesAsync();

            result.Should().HaveCount(0);
        }

        [Test]
        public async Task GetFileAsync_ThrowNorFoundExc_ThenCalledToUnknownFile()
        {
            using var sut = await _sutFactory.CreateAsync();
            const string key = "smth";

            Func<Task> act = sut.Awaiting(x => x.GetFileAsync(key));

            await act.Should().ThrowAsync<FileNotFoundException>()
                .WithMessage("Not found file with key=smth in bucket=test");
        }

        [Test]
        public async Task GetFile_ReturnValidStream_AfterCalled()
        {
            await using var fileStream = new FileStream(Path.GetTempFileName(),
                FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite,
                4096, FileOptions.RandomAccess);
            using var sut = await _sutFactory.CreateAsync();
            var key = fileStream.Name;
            var data = new byte[8];
            data[1] = 15;
            fileStream.Write(data);

            await sut.SaveFileAsync(key, fileStream);

            var response = await sut.GetFile(key);
            var dataResponse = new byte[response.Length];
            response.Read(dataResponse);

            dataResponse.Should().Equal(data);
        }
        
        [Test]
        public async Task GetFileAsync_ReturnFile_ThenCalled()
        {
            await using var fileStream = new FileStream(Path.GetTempFileName(),
                FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None,
                4096, FileOptions.RandomAccess | FileOptions.DeleteOnClose);
            using var sut = await _sutFactory.CreateAsync();
            var key = fileStream.Name;
            var fileName = "superName";
            fileStream.Write(new byte[8]);

            await sut.SaveFileAsync(key, fileStream);

            var result = await sut.GetFileAsync(key, fileName);

            result.Key.Should().Be(key);
            result.DownloadLink.Should().Contain(fileName);
        }
    }
}