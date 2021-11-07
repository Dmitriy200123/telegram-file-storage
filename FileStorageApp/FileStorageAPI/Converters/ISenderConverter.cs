using System.Collections.Generic;
using FileStorageAPI.Models;
using FileStorageApp.Data.InfoStorage.Models;

namespace FileStorageAPI.Converters
{
    public interface ISenderConverter
    {
        Sender ConvertFileSender(FileSender fileSender);
        List<Sender> ConvertFileSenders(List<FileSender> fileSender);
    }
}