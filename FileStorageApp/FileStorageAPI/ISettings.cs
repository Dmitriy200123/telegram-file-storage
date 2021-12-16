using FileStorageApp.Data.InfoStorage.Config;
using Microsoft.Extensions.Configuration;

namespace FileStorageAPI
{
    public interface ISettings
    {
        IConfiguration Configuration { get; }
        byte[] Key { get; }
        IDataBaseConfig DataBaseConfig { get; }
    }
}