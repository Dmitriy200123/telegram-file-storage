using System;

namespace FileStorageApp.Data.InfoStorage.Tables.FileSender
{
    public interface IFileSenderTable
    {
        Models.FileSender GetFileSenderById(Guid id);
    }
}