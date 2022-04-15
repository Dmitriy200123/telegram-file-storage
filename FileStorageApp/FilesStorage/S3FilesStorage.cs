using System;
using System.Collections.Generic;
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
        private bool _disposed = false;

        public S3FilesStorage(IAmazonS3 client, IS3FilesStorageOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _s3Client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task SaveFileAsync(string key, Stream stream)
        {
            var request = new PutObjectRequest
            {
                BucketName = _options.BucketName,
                CannedACL = _options.Permission,
                Key = key,
                InputStream = stream,
            };

            await _s3Client.PutObjectAsync(request);
        }

        public async Task<File> GetFileAsync(string key, string fileName = null)
        {
            var existingFiles = await this.GetFilesAsync();
            if (existingFiles.All(x => x.Key != key))
                throw new FileNotFoundException($"Not found file with key={key} in bucket={_options.BucketName}");


            return new File(GetDownloadStringFromKey(key, fileName).Replace("s3", "localhost"), key);
        }

        public async Task<Stream> GetFileStreamAsync(string key)
        {
            var request = new GetObjectRequest
            {
                BucketName = _options.BucketName,
                Key = key
            };
            var response = await _s3Client.GetObjectAsync(request);
            return response.ResponseStream;
        }

        private string GetDownloadStringFromKey(string key, string fileName = null)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _options.BucketName,
                Key = key,
                Expires = DateTime.UtcNow.Add(_options.FileDownloadLinkTtl)
            };
            if (fileName != null)
            {
                request.ResponseHeaderOverrides.ContentDisposition = $"attachment; filename={fileName}";
            }


            return _s3Client.GetPreSignedURL(request);
        }

        public async Task DeleteFileAsync(string key)
        {
            var deleteObjectRequest = new DeleteObjectRequest
            {
                BucketName = _options.BucketName,
                Key = key
            };

            await _s3Client.DeleteObjectAsync(deleteObjectRequest);
        }

        public async Task<IEnumerable<File>> GetFilesAsync()
        {
            var response = await _s3Client.ListObjectsAsync(new ListObjectsRequest()
                {BucketName = _options.BucketName});

            var result = response.S3Objects.Select(
                x => new File(GetDownloadStringFromKey(x.Key), x.Key)
            );

            return result;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _s3Client.Dispose();
            }

            _disposed = true;
        }
    }
}