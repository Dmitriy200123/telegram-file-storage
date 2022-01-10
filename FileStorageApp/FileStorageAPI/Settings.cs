using FileStorageApp.Data.InfoStorage.Config;
using Microsoft.Extensions.Configuration;

namespace FileStorageAPI
{
    public static class Settings
    {
        public static IConfiguration Configuration { get; private set; }
        public static byte[] Key { get; private set; }

        public static IDataBaseConfig DataBaseConfig { get; private set; }

        public static void SetUpSettings(IConfiguration configuration, byte[] key, IDataBaseConfig dataBaseConfig)
        {
            Configuration = configuration;
            Key = key;
            DataBaseConfig = dataBaseConfig;
        }
    }
}