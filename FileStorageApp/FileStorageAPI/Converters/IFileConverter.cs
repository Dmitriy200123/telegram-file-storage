using System.Collections.Generic;
using FileStorageAPI.Models;

namespace FileStorageAPI.Converters
{
    public interface IFileConverter
    {
        File ConvertFile(FileStorageApp.Data.InfoStorage.Models.File file);
    }
}