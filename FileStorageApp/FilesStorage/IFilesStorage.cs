using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;

namespace FilesStorage
{
    public interface IFilesStorage
    {
        Task SaveFileAsync(string key, FileStream stream);

        Task GetFileAsync(string key);
    }
    public interface IS3FilesStorageOptions
    {
        string AccessKey { get; }
        string SecretKey { get; }
        string DefautBucketName { get; }
        AmazonS3Config Config { get; }
        S3CannedACL DefaultPermission { get; }
    }

    public interface IFilesStorageFactory
    {
        Task<IFilesStorage> CreateAsync(IS3FilesStorageOptions options);

    }
}