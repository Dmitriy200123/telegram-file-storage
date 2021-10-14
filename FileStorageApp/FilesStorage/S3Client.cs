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
        public S3Client(string accessKey, string secretKey, string bucket, AmazonS3Config config)
        {
            accessKey = accessKey ?? throw new ArgumentNullException(nameof(accessKey));
            secretKey = secretKey ?? throw new ArgumentNullException(nameof(secretKey));
            _bucket = bucket ?? throw new ArgumentNullException(nameof(bucket));

            _s3Client = new AmazonS3Client(
                accessKey,
                secretKey,
                config
            );
        }

        private static string _bucket;
        private readonly IAmazonS3 _s3Client;

        public async Task<PutObjectResponse> Save(string key, FileStream stream, S3CannedACL accessFlag)
        {
            var request = new PutObjectRequest
            {
                BucketName = _bucket,
                CannedACL = accessFlag,
                Key = key,
                InputStream = stream
            };

            var response = await _s3Client.PutObjectAsync(request);
            return response;
        }


        public async Task<GetObjectResponse> GetFile(string key)
        {
            var response = await _s3Client.GetObjectAsync(
                new GetObjectRequest()
                {
                    BucketName = _bucket,
                    Key = key
                }
            );
            return response;
        }

        public async Task<ListObjectsResponse> GetFiles()
        {
            var response = await _s3Client.ListObjectsAsync(new ListObjectsRequest() {BucketName = _bucket});
            return response;
        }
    }
}