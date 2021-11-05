using FileStorageAPI.Models;
using FileStorageAPI.Providers;
using DataBaseFile = FileStorageApp.Data.InfoStorage.Models.File;
namespace FileStorageAPI.Converters
{
    public class FileConverter : IFileConverter
    {
        private readonly IDownloadLinkProvider downloadLinkProvider;
        private readonly IChatConverter chatConverter;
        private readonly ISenderConverter senderConverter;

        public FileConverter(IDownloadLinkProvider downloadLinkProvider, IChatConverter chatConverter, ISenderConverter senderConverter)
        {
            this.downloadLinkProvider = downloadLinkProvider;
            this.chatConverter = chatConverter;
            this.senderConverter = senderConverter;
        }
        public File ConvertFile(DataBaseFile file)
        {
            return new File
            {
                FileId = file.Id,
                FileName = file.Name,
                FileType = file.Type,
                Sender = senderConverter.ConvertFileSender(file.FileSender),
                UploadDate = file.UploadDate,
                Chat = chatConverter.ConvertChat(file.Chat),
                DownloadLink = downloadLinkProvider.GetDownloadLink()
            };
        }
    }
}