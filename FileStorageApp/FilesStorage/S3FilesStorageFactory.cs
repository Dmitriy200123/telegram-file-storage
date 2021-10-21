using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using FilesStorage.Interfaces;

namespace FilesStorage
{
    public class S3FilesStorageFactory : IFilesStorageFactory
    {
        private readonly IS3FilesStorageOptions Options;

        public S3FilesStorageFactory(IS3FilesStorageOptions options)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<IFilesStorage> CreateAsync()
        {
            var client = new AmazonS3Client(
                Options.AccessKey,
                Options.SecretKey,
                Options.Config
            );

            var buckets = await client.ListBucketsAsync();

            if (buckets.Buckets.All(bucket => bucket.BucketName != Options.BucketName))
            {
                await client.PutBucketAsync(new PutBucketRequest
                {
                    BucketName = Options.BucketName,
                    UseClientRegion = true
                });
            }

            return new S3FilesStorage(client, Options);
        }
    }
}