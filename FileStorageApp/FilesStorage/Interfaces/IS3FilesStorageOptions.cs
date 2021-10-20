using System;
using Amazon.S3;

namespace FilesStorage.Interfaces
{
    public interface IS3FilesStorageOptions
    {
        string AccessKey { get; }
        string SecretKey { get; }
        string BucketName { get; }
        AmazonS3Config Config { get; }
        S3CannedACL Permission { get; }
        double FileDownloadLinkTtlInHours { get; }
    }
}