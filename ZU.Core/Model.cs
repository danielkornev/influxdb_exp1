using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ZU.Core.Developer;

namespace ZU.Storage
{
    

    public class Model : IModel
    {
        public string Name { get; }
        

        public Model(string modelName, IStorageContext ctx)
        {
            this.StorageContext = ctx;
            this.Name = modelName;

            this.Entities = new ObservableCollection<IEntity>();
        }

        public IStorageContext StorageContext { get; set; }

        public async static Task<Model> GetOrCreateModel(string modelName, StorageContext ctx)
        {
            var exists = await ctx.StorageProvider.StoreExists(modelName);

            if (!exists)
            {
                exists = await ctx.StorageProvider.CreateStoreIfNotExists(modelName);

                if (!exists)
                    throw new Exception("Failed to create new database with a given name");
            }

            Model model = new Model(modelName, ctx);

            return model;
        }

        public async Task AddEntity(IEntity entity)
        {
            await this.StorageContext.AddEntity(this, entity);

            this.Entities.Add(entity);
        }

        public ObservableCollection<IEntity> Entities { get; private set; }
    }
}