using System;
using FileStorageApp.Data.InfoStorage.Models;

namespace FileStorageApp.Data.InfoStorage.Tables.Files
{
    public interface IFilesTable : IDisposable
    {
        File GetFileById(Guid id);
    }
}