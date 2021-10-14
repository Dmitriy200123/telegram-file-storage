using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;

namespace FilesStorage
{
    public interface IFilesStorage
    {
        Task<PutObjectResponse> SaveAsync(string key, FileStream stream, S3CannedACL accessFlag,
            string bucketName = null);

        Task<GetObjectResponse> GetFileAsync(string key);
    }
}