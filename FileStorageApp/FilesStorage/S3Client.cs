using System;
using System.IO;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

namespace FilesStorage
{
    public class S3Client : IFilesStorage
    {
        public S3Client(string accessKey, string secretKey, string bucket)
        {
            accessKey = accessKey ?? throw new ArgumentNullException(nameof(accessKey));
            secretKey = secretKey ?? throw new ArgumentNullException(nameof(secretKey));
            _bucket = bucket ?? throw new ArgumentNullException(nameof(bucket));

            _s3Client = AWSClientFactory.CreateAmazonS3Client(accessKey, secretKey);
        }

        private static string _bucket;
        private readonly IAmazonS3 _s3Client;

        public void Save(string key, FileStream stream, S3CannedACL accessFlag)
        {
            var request = new PutObjectRequest
            {
                BucketName = _bucket,
                CannedACL = accessFlag,
                Key = key,
                InputStream = stream
            };

            _s3Client.PutObject(request);
        }


        public GetObjectResponse GetFile(string key)
        {
            return _s3Client.GetObject(
                new GetObjectRequest()
                {
                    BucketName = _bucket,
                    Key = key
                }
            );
        }

        public ListObjectsResponse GetFiles()
        {
            return _s3Client.ListObjects(new ListObjectsRequest() {BucketName = _bucket});
        }
    }
}