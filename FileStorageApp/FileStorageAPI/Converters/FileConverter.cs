using FileStorageAPI.Models;
using FileStorageAPI.Providers;
using DataBaseFile = FileStorageApp.Data.InfoStorage.Models.File;

namespace FileStorageAPI.Converters
{
    /// <inheritdoc />
    public class FileConverter : IFileConverter
    {
        private readonly IDownloadLinkProvider downloadLinkProvider;
        private readonly IChatConverter chatConverter;
        private readonly ISenderConverter senderConverter;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="downloadLinkProvider"></param>
        /// <param name="chatConverter"></param>
        /// <param name="senderConverter"></param>
        public FileConverter(IDownloadLinkProvider downloadLinkProvider, IChatConverter chatConverter,
            ISenderConverter senderConverter)
        {
            this.downloadLinkProvider = downloadLinkProvider;
            this.chatConverter = chatConverter;
            this.senderConverter = senderConverter;
        }

        /// <inheritdoc />
        public File ConvertFile(DataBaseFile file)
        {
            return new File
            {
                FileId = file.Id,
                FileName = file.Name,
                FileType = file.Type,
                Sender = senderConverter.ConvertFileSender(file.FileSender),
                UploadDate = file.UploadDate,
                Chat = chatConverter.ConvertToChatInApi(file.Chat),
                DownloadLink = downloadLinkProvider.GetDownloadLink(file.Id).Result
            };
        }
    }
}