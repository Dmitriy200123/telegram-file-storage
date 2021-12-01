using FileStorageApp.Data.InfoStorage.Factories;

namespace DataBaseFiller.Actions
{
    public interface IAction
    {
        void DoAction(IInfoStorageFactory infoStorageFactory);
    }
}