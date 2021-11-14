using System;
using System.Threading.Tasks;
using FilesStorage.Interfaces;

namespace FileStorageAPI.Providers
{
    /// <inheritdoc />
    public class DownloadLinkProvider : IDownloadLinkProvider
    {
        private readonly FilesStorage.Interfaces.IFilesStorage _physicalFilesStorage;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="physicalFilesStorage"></param>
        public DownloadLinkProvider(IFilesStorage physicalFilesStorage)
        {
            _physicalFilesStorage = physicalFilesStorage;
        }
        
        /// <inheritdoc />
        public async Task<string> GetDownloadLink(Guid id)
        {
            var file = await _physicalFilesStorage.GetFileAsync(id.ToString());
            return file.DownloadLink;
        }
    }
}