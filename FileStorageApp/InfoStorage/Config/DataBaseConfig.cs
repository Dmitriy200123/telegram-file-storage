namespace FileStorageApp.Data.InfoStorage.Config
{
    public class DataBaseConfig : IDataBaseConfig
    {
        public string ConnectionString { get; }

        public DataBaseConfig(string connectionString)
        {
            ConnectionString = connectionString;
        }
    }
}