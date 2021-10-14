using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;

namespace FilesStorage
{
    public interface IFilesStorage
    {
        Task<PutObjectResponse> Save(string key, FileStream stream, S3CannedACL accessFlag);

        Task<GetObjectResponse> GetFile(string key);
    }
}