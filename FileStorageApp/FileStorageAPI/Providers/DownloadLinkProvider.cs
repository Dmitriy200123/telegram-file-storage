using System;
using System.Threading.Tasks;
using FilesStorage.Interfaces;

namespace FileStorageAPI.Providers
{
    /// <inheritdoc />
    public class DownloadLinkProvider : IDownloadLinkProvider
    {
        private readonly IFilesStorageFactory _physicalFilesStorage;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filesStorageFactory">Фабрика для получения доступа к физическому хранилищу чатов</param>
        public DownloadLinkProvider(IFilesStorageFactory filesStorageFactory)
        {
            _physicalFilesStorage = filesStorageFactory;
        }
        
        /// <inheritdoc />
        public async Task<string> GetDownloadLink(Guid id)
        {
            var fileStorage = await _physicalFilesStorage.CreateAsync();
            var file = await fileStorage.GetFileAsync(id.ToString());
            return file.DownloadLink;
        }
    }
}