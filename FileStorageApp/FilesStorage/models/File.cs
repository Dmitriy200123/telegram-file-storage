using System;

namespace FilesStorage.models
{
    public class File
    {
        public string DownloadLink { get; }

        public File(string downloadLink)
        {
            DownloadLink = downloadLink ?? throw new ArgumentNullException(nameof(downloadLink));
        }
    }
}