using System.Threading.Tasks;
using ZU.Storage;

namespace ZU.Core.Developer
{
    public interface IStorageContext
    {
        IStorageProvider StorageProvider { get; }

        Task AddEntity(IModel model, IEntity entity);
    }
}