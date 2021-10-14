using System;

namespace FileStorageApp.Data.InfoStorage.Tables.Files
{
    public interface IFilesTable
    {
        Models.File GetFileById(Guid id);
    }
}