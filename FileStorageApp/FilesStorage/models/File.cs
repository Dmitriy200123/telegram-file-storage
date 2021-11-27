using System;

namespace FilesStorage.models
{
    public class File
    {
        public string DownloadLink { get; }
        public string Key { get; }


        public File(string downloadLink, string key, string name = "")
        {
            DownloadLink = downloadLink ?? throw new ArgumentNullException(nameof(downloadLink));
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }
    }
}