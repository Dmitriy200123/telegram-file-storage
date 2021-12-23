using System.Linq;
using FileStorageApp.Data.InfoStorage.Config;
using FileStorageApp.Data.InfoStorage.Models;

namespace FileStorageApp.Data.InfoStorage.Storages.MarkedTextsTags
{
    internal class MarkedTextTagsStorage : BaseStorage<MarkedTextTags>, IMarkedTextTagsStorage
    {
        internal MarkedTextTagsStorage(IDataBaseConfig dataBaseConfig) : base(dataBaseConfig)
        {
        }

        public MarkedTextTags FirstOrDefault() => DbSet.FirstOrDefault();
    }
}