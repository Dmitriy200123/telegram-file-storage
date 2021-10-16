namespace FileStorageApp.Data.InfoStorage.Config
{
    internal interface IDataBaseConfig
    {
        string GetConnectionString();
        void SetConnectionString(string connectionString);
    }
}