using System.Collections.Generic;
using System.Threading.Tasks;
using ZU.Storage;

namespace ZU.Core.Developer
{
    public interface IStorageProvider
    {
        void Instantiate(Dictionary<string, string> settings);
        bool IsInstantiated { get; }
        bool IsConnected { get; }
        Task<bool> StoreExists(string storeName);
        Task<bool> CreateStoreIfNotExists(string storeName);
        Task AddEntity(IModel model, IEntity entity);
        Task ClearAllStores();
    }
}