using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZU.Core.Developer;

namespace ZU.Storage
{
    public class StorageContext : IStorageContext
    {
        public StorageContext(IStorageProvider provider)
        {
            this.StorageProvider = provider;
        }

        public IStorageProvider StorageProvider { get; }

        public async Task AddEntity(IModel model, IEntity entity)
        {
            await this.StorageProvider.AddEntity(model, entity);
        }
    }
}
