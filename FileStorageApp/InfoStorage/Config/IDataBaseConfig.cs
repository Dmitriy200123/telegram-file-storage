namespace FileStorageApp.Data.InfoStorage.Config
{
    public interface IDataBaseConfig
    {
        string GetConnectionString();

        void SetConnectionString(string connectionString);
    }
}