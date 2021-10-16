using System;
using FileStorageApp.Data.InfoStorage.Models;

namespace FileStorageApp.Data.InfoStorage.Tables.Files
{
    public interface IFilesTable
    {
        File GetFileById(Guid id);
    }
}