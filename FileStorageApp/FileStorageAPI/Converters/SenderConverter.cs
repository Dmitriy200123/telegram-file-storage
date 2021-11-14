using System.Collections.Generic;
using System.Linq;
using FileStorageAPI.Models;
using FileStorageApp.Data.InfoStorage.Models;

namespace FileStorageAPI.Converters
{
    /// <inheritdoc />
    public class SenderConverter : ISenderConverter
    {
        /// <inheritdoc />
        public Sender ConvertFileSender(FileSender fileSender)
        {
            if (fileSender == null)
                return null;
            return new Sender
            {
                UserId = fileSender.Id,
                TelegramName = fileSender.TelegramUserName,
                FullName = fileSender.FullName,
            };
        }

        /// <inheritdoc />
        public List<Sender> ConvertFileSenders(List<FileSender> fileSender)
        {
            return fileSender.Select(ConvertFileSender).ToList();
        }
    }
}