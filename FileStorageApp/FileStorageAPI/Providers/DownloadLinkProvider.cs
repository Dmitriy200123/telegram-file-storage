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
        /// Инициализирует новый экземпляр класса <see cref="DownloadLinkProvider"/>.
        /// </summary>
        /// <param name="filesStorageFactory">Фабрика для получения доступа к физическому хранилищу чатов</param>
        public DownloadLinkProvider(IFilesStorageFactory filesStorageFactory)
        {
            _physicalFilesStorage = filesStorageFactory ?? throw new ArgumentNullException(nameof(filesStorageFactory));
        }

        /// <inheritdoc />
        public async Task<string?> GetDownloadLinkAsync(Guid id, string name)
        {
            var fileStorage = await _physicalFilesStorage.CreateAsync();
            try
            {
                var file = await fileStorage.GetFileAsync(id.ToString());
                return file.DownloadLink;
            }
            catch
            {
                return null;
            }
        }
    }
}