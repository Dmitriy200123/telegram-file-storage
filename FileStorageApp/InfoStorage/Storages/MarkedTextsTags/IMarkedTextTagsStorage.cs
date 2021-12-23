using System;
using FileStorageApp.Data.InfoStorage.Models;

namespace FileStorageApp.Data.InfoStorage.Storages.MarkedTextsTags
{
    public interface IMarkedTextTagsStorage : IDisposable, IInfoStorage<MarkedTextTags>
    {
        MarkedTextTags? FirstOrDefault();
    }
}