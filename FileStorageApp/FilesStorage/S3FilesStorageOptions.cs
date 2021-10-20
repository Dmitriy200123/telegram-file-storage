using System;
using FilesStorage.Interfaces;
using Amazon.S3;

namespace FilesStorage
{
    public class S3FilesStorageOptions : IS3FilesStorageOptions
    {
        public string AccessKey { get; }
        public string SecretKey { get; }
        public string BucketName { get; }
        public AmazonS3Config Config { get; }
        public S3CannedACL Permission { get; }
        public double FileDownloadLinkTtlInHours { get; }

        public S3FilesStorageOptions(string accessKey, string secretKey, string bucketName, AmazonS3Config config,
            S3CannedACL permission, double fileDownloadLinkTtlInHours)
        {
            AccessKey = accessKey ?? throw new ArgumentNullException(nameof(accessKey));
            SecretKey = secretKey ?? throw new ArgumentNullException(nameof(secretKey));
            BucketName = bucketName ?? throw new ArgumentNullException(nameof(bucketName));
            Config = config ?? throw new ArgumentNullException(nameof(config));
            Permission = permission ?? throw new ArgumentNullException(nameof(permission));
            FileDownloadLinkTtlInHours = fileDownloadLinkTtlInHours;
        }
    }
}