using System.IO;
using Amazon.S3;
using Amazon.S3.Model;

namespace FilesStorage
{
    public interface IFilesStorage
    {
        void Save(string key, FileStream stream, S3CannedACL accessFlag);

        GetObjectResponse GetFile(string key);
    }
}