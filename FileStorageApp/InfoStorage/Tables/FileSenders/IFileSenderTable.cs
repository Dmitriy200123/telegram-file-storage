using System;
using FileStorageApp.Data.InfoStorage.Models;

namespace FileStorageApp.Data.InfoStorage.Tables.FileSenders
{
    public interface IFileSenderTable
    {
        FileSender GetFileSenderById(Guid id);
    }
}