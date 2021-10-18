using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

namespace FilesStorage
{
    public class S3FilesStorageOptions : IS3FilesStorageOptions
    {
        public S3FilesStorageOptions(string accessKey, string secretKey, string defautBucketName, AmazonS3Config config,
            S3CannedACL defaultPermission)
        {
            AccessKey = accessKey ?? throw new ArgumentNullException(nameof(accessKey));
            SecretKey = secretKey ?? throw new ArgumentNullException(nameof(secretKey));
            DefautBucketName = defautBucketName ?? throw new ArgumentNullException(nameof(defautBucketName));
            Config = config ?? throw new ArgumentNullException(nameof(config));
            DefaultPermission = defaultPermission ?? throw new ArgumentNullException(nameof(defaultPermission));
        }

        public string AccessKey { get; }
        public string SecretKey { get; }
        public string DefautBucketName { get; }
        public AmazonS3Config Config { get; }
        public S3CannedACL DefaultPermission { get; }
    }

    public class S3FileStorage : IFilesStorage
    {
        private IS3FilesStorageOptions options;
        private readonly IAmazonS3 _s3Client;

        public S3FileStorage(IAmazonS3 client, IS3FilesStorageOptions options)
        {
            this.options = options ?? throw new ArgumentNullException(nameof(options));

            _s3Client = client;
        }

        public async Task SaveFileAsync(string key, FileStream stream)
        {
            var request = new PutObjectRequest
            {
                BucketName = options.DefautBucketName,
                CannedACL = options.DefaultPermission,
                Key = key,
                InputStream = stream
            };

            await _s3Client.PutObjectAsync(request);
        }

        public async Task GetFileAsync(string key)
        {
            await _s3Client.GetObjectAsync(
                new GetObjectRequest()
                {
                    BucketName = options.DefautBucketName,
                    Key = key
                }
            );
        }

        public async Task<ListObjectsResponse> GetFilesAsync()
        {
            var response = await _s3Client.ListObjectsAsync(new ListObjectsRequest()
                {BucketName = options.DefautBucketName});
            return response;
        }
    }

    public class S3FilesStorageFactory : IFilesStorageFactory
    {
        public async Task<IFilesStorage> CreateAsync(IS3FilesStorageOptions options)
        {
            var client = new AmazonS3Client(
                options.AccessKey,
                options.SecretKey,
                options.Config
            );
            var buckets = await client.ListBucketsAsync();
            if (buckets.Buckets.Count(bucket => bucket.BucketName == options.DefautBucketName) == 0)
            {
                await client.PutBucketAsync(new PutBucketRequest
                {
                    BucketName = options.DefautBucketName,
                    UseClientRegion = true
                });
            }

            return new S3FileStorage(client, options);
        }
    }
}