using System.Threading.Tasks;
using NUnit.Framework;
using Amazon.S3;
using FilesStorage;

namespace FilesStorageTests
{
    public class Tests
    {
        private const string serviceUrl = "http://localhost:4566";
        private S3Client testClient;

        [SetUp]
        public void Setup()
        {
            var config = new AmazonS3Config();
            config.ServiceURL = serviceUrl;
            config.ForcePathStyle = true;
            testClient = new S3Client("123", "123", "mytestbucket", config);
            Task.Run(async () =>
            {
                var buckets = await testClient.GetBucketsAsync();
                foreach (var bucket in buckets.Buckets)
                {
                    await testClient.DeleteBucket(bucket.BucketName);
                }
            }).GetAwaiter().GetResult();
        }

        [TearDown]
        public void TearDown()
        {
            Task.Run(async () =>
            {
                var buckets = await testClient.GetBucketsAsync();
                foreach (var bucket in buckets.Buckets)
                {
                    await testClient.DeleteBucket(bucket.BucketName);
                }
            }).GetAwaiter().GetResult();
        }

        [Test]
        public async Task TestCreateBucket()
        {
            var testName = "abc";
            await testClient.CreateBucket(testName);
            var buckets = await testClient.GetBucketsAsync();
            foreach (var bucket in buckets.Buckets)
            {
                if (bucket.BucketName == testName)
                {
                    Assert.Pass();
                }
            }

            Assert.Fail();
        }
    }
}