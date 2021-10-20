using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using FilesStorage.Interfaces;
using File = FilesStorage.models.File;

namespace FilesStorage
{
    public class S3FilesStorage : IFilesStorage
    {
        private readonly IS3FilesStorageOptions _options;
        private readonly IAmazonS3 _s3Client;

        public S3FilesStorage(IAmazonS3 client, IS3FilesStorageOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _s3Client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task SaveFileAsync(string key, FileStream stream)
        {
            var request = new PutObjectRequest
            {
                BucketName = _options.BucketName,
                CannedACL = _options.Permission,
                Key = key,
                InputStream = stream
            };

            await _s3Client.PutObjectAsync(request);
        }

        public File GetFile(string key)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _options.BucketName,
                Key = key,
                Expires = DateTime.UtcNow.AddHours(_options.FileDownloadLinkTtlInHours)
            };
            var urlString = _s3Client.GetPreSignedURL(request);

            return new File(urlString);
        }

        public async Task<ListObjectsResponse> GetFilesAsync()
        {
            var response = await _s3Client.ListObjectsAsync(new ListObjectsRequest()
                {BucketName = _options.BucketName});
            return response;
        }
    }
}