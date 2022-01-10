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
        private readonly IS3FilesStorageOptions _options;

        public S3FilesStorageFactory(IS3FilesStorageOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<IFilesStorage> CreateAsync()
        {
            var client = new AmazonS3Client(
                _options.AccessKey,
                _options.SecretKey,
                _options.Config
            );

            var buckets = await client.ListBucketsAsync();

            if (buckets.Buckets.All(bucket => bucket.BucketName != _options.BucketName))
            {
                await client.PutBucketAsync(new PutBucketRequest
                {
                    BucketName = _options.BucketName,
                    UseClientRegion = true
                });
            }

            return new S3FilesStorage(client, _options);
        }
    }
}