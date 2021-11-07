using System.Collections.Generic;
using System.Linq;
using FileStorageAPI.Models;
using FileStorageApp.Data.InfoStorage.Models;

namespace FileStorageAPI.Converters
{
    public class SenderConverter : ISenderConverter
    {
        public Sender ConvertFileSender(FileSender fileSender)
        {
            return new Sender
            {
                UserId = fileSender.Id,
                TelegramName = fileSender.TelegramUserName,
                FullName = fileSender.FullName,
            };
        }

        public List<Sender> ConvertFileSenders(List<FileSender> fileSender)
        {
            return fileSender.Select(ConvertFileSender).ToList();
        }
    }
}