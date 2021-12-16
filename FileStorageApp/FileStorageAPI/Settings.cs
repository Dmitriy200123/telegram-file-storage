using FileStorageApp.Data.InfoStorage.Config;
using Microsoft.Extensions.Configuration;

namespace FileStorageAPI
{
    public class Settings : ISettings
    {
        public Settings(IConfiguration configuration, byte[] key, IDataBaseConfig dataBaseConfig)
        {
            Configuration = configuration;
            Key = key;
            DataBaseConfig = dataBaseConfig;
        }

        public IConfiguration Configuration { get; }
        public byte[] Key { get; }
        
        public IDataBaseConfig DataBaseConfig { get; }
    }
}