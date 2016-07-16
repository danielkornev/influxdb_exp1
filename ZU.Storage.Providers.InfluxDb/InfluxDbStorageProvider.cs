using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdysTech.InfluxDB.Client.Net;
using ZU.Core.Developer;

namespace ZU.Storage.Providers.InfluxDb
{
    public class InfluxDbStorageProvider : IStorageProvider
    {
        internal InfluxDBClient DataContext { get; private set; }

        public bool IsInstantiated { get; private set; }

        public bool IsConnected => DataContext != null; 

        /// <summary>
        /// Passes anonymous type. Expects:
        /// 
        /// string NetworkPath
        /// string UserName
        /// string Password
        /// </summary>
        /// <param name="settings"></param>
        public void Instantiate(Dictionary<string, string> settings)
        {
            if (IsInstantiated) throw new Exception("This Storage Provider has been already instantiated.");

            try
            {
                DataContext = new InfluxDBClient(settings["NetworkPath"], settings["UserName"], settings["Password"]);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to complete instation of the InfluxDbStorageProvider due to:\n" + ex.Message);
            }

            // we are instantiated now
            this.IsInstantiated = true;

        }

        public async Task<bool> StoreExists(string storeName)
        {
            var dbNames = await DataContext.GetInfluxDBNamesAsync();

            return dbNames.Any(db => db == storeName);
        }

        public async Task<bool> CreateStoreIfNotExists(string storeName)
        {
            return await DataContext.CreateDatabaseAsync(storeName);
        }

        public async Task AddEntity(IModel model, IEntity entity)
        {
            var entities = new List<IInfluxDatapoint>();

            var entityPoint = new InfluxDatapoint<double> {UtcTimestamp = entity.TLChange};
            entityPoint.Tags.Add("Id", entity.Id);
            entityPoint.Tags.Add("Title", entity.Title);
            entityPoint.Fields.Add("TLBirth", entity.TLBirth.Ticks);
            entityPoint.Fields.Add("TLChange", entity.TLChange.Ticks);
            entityPoint.Fields.Add("TLDeath", entity.TLDeath.Ticks);
            entityPoint.MeasurementName = "Entities";
            entityPoint.Precision = TimePrecision.Nanoseconds;
            entities.Add(entityPoint);

            var r = await this.DataContext.PostPointsAsync(model.Name, entities);
        }

        public async Task ClearAllStores()
        {
            var allDbNames = await this.DataContext.GetInfluxDBNamesAsync();

            foreach (var dbName in allDbNames)
            {
                if (dbName == "_internal") continue;

                await this.DataContext.QueryDBAsync("", "DROP DATABASE " + dbName);
            }
        }
    }
}