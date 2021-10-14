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
        private static string _defaultBucket;
        private readonly IAmazonS3 _s3Client;

        public S3Client(string accessKey, string secretKey, string defautBucket, AmazonS3Config config)
        {
            accessKey = accessKey ?? throw new ArgumentNullException(nameof(accessKey));
            secretKey = secretKey ?? throw new ArgumentNullException(nameof(secretKey));
            _defaultBucket = defautBucket ?? throw new ArgumentNullException(nameof(defautBucket));

            _s3Client = new AmazonS3Client(
                accessKey,
                secretKey,
                config
            );
        }

        public async Task<PutObjectResponse> SaveAsync(string key, FileStream stream,
            S3CannedACL accessFlag, string bucketName = null)
        {
            var request = new PutObjectRequest
            {
                BucketName = bucketName ?? _defaultBucket,
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
                    BucketName = _defaultBucket,
                    Key = key
                }
            );
            return response;
        }

        public async Task<ListObjectsResponse> GetFilesAsync()
        {
            var response = await _s3Client.ListObjectsAsync(new ListObjectsRequest() {BucketName = _defaultBucket});
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

        public async Task<ListBucketsResponse> GetBuckets()
        {
            var response = await _s3Client.ListBucketsAsync();
            return response;
        }
    }
}