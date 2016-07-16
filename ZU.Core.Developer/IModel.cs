using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ZU.Core.Developer;

namespace ZU.Storage
{
    public interface IModel
    {
        string Name { get; }
        IStorageContext StorageContext { get; set; }
        ObservableCollection<IEntity> Entities { get; }
        Task AddEntity(IEntity entity);
    }
}