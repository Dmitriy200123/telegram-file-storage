using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Factories;

namespace DataBaseFiller.Actions
{
    public interface IAction
    {
        Task DoActionAsync(IInfoStorageFactory infoStorageFactory);
    }
}