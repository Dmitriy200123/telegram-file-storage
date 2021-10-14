using System;
using System.IO;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

namespace FilesStorage
{
    public class S3Client : IFilesStorage
    {
        private static string _defaultBucketName;
        private readonly IAmazonS3 _s3Client;

        public S3Client(string accessKey, string secretKey, string defautBucket, AmazonS3Config config)
        {
            accessKey = accessKey ?? throw new ArgumentNullException(nameof(accessKey));
            secretKey = secretKey ?? throw new ArgumentNullException(nameof(secretKey));
            _defaultBucketName = defautBucket ?? throw new ArgumentNullException(nameof(defautBucket));

            _s3Client = new AmazonS3Client(
                accessKey,
                secretKey,
                config
            );

            CreateDefaultBucket();
        }

        private void CreateDefaultBucket()
        {
            ListBucketsResponse buckets = new ListBucketsResponse();
            Task.Run(async () => { buckets = await this.GetBucketsAsync(); }).GetAwaiter().GetResult();

            foreach (var bucket in buckets.Buckets)
            {
                if (_defaultBucketName == bucket.BucketName)
                {
                    return;
                }
            }

            Task.Run(async () => { await this.CreateBucket(_defaultBucketName); }).GetAwaiter().GetResult();
        }

        public async Task<PutObjectResponse> SaveAsync(string key, FileStream stream,
            S3CannedACL accessFlag, string bucketName = null)
        {
            var request = new PutObjectRequest
            {
                BucketName = bucketName ?? _defaultBucketName,
                CannedACL = accessFlag,
                Key = key,
                InputStream = stream
            };

            var response = await _s3Client.PutObjectAsync(request);
            return response;
        }


        public async Task<GetObjectResponse> GetFileAsync(string key)
        {
            var response = await _s3Client.GetObjectAsync(
                new GetObjectRequest()
                {
                    BucketName = _defaultBucketName,
                    Key = key
                }
            );
            return response;
        }

        public async Task<ListObjectsResponse> GetFilesAsync()
        {
            var response = await _s3Client.ListObjectsAsync(new ListObjectsRequest() {BucketName = _defaultBucketName});
            return response;
        }

        public async Task<PutBucketResponse> CreateBucket(string name)
        {
            var response = await _s3Client.PutBucketAsync(new PutBucketRequest
            {
                BucketName = name,
                UseClientRegion = true
            });
            return response;
        }

        public async Task<DeleteBucketResponse> DeleteBucket(string name)
        {
            var response = await _s3Client.DeleteBucketAsync(
                new DeleteBucketRequest()
                {
                    BucketName = name
                }
            );
            return response;
        }

        public async Task<ListBucketsResponse> GetBucketsAsync()
        {
            var response = await _s3Client.ListBucketsAsync();
            return response;
        }
    }
}