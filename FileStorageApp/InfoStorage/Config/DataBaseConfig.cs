namespace FileStorageApp.Data.InfoStorage.Config
{
    public class DataBaseConfig : IDataBaseConfig
    {
        private string _connectionString;

        public void SetConnectionString(string connectionString)
        {
            _connectionString = connectionString;
        }
        public string GetConnectionString()
        {
            return _connectionString;
        }
    }
}