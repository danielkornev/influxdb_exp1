using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AdysTech.InfluxDB.Client.Net;
using NUnit.Framework;
using ZU.Core.Developer;
using ZU.Framework;
using ZU.Storage;

namespace ZU.StorageTests
{
    [TestFixture]
    public class StorageContextTest
    {
        [Test]
        public async Task InitStorageTest()
        {
            var currentDirectory = TestContext.CurrentContext.TestDirectory;

            // Step 0
            // Looking for all StorageProvider libraries
            var storageProviderAssemblies =
                new DirectoryInfo(currentDirectory).GetFiles("*StorageProvider.dll")
                    //.Where(f => f.Extension.ToLowerInvariant() == "dll")
                    .ToList();

            // checking that there is at least one such library
            Assert.AreEqual(true, storageProviderAssemblies.Count > 0);

            // Step 1
            int foundProviderTypes = 0;
            // Loading assemblies
            foreach (var asmFile in storageProviderAssemblies)
            {
                var asm = AssemblyLoader.TryToIdentifyAndLoadAssembly(asmFile.FullName, typeof(IStorageProvider));

                var allStorageProviderTypes = asm.GetTypesWithInterface(typeof(IStorageProvider)).ToList();

                Assert.AreEqual(true, allStorageProviderTypes.Count > 0);

                // increasing number
                foundProviderTypes += allStorageProviderTypes.Count;

                // registering all found providers
                foreach (var providerType in allStorageProviderTypes)
                {
                    StorageProviderFabric.RegisterIStorageProvider(providerType.FullName, providerType);
                }
            }

            // checking that all found providers have been registered
            Assert.AreEqual(foundProviderTypes, StorageProviderFabric.StorageProviders.Count);

            // Step 2
            // Preparing provider
            string _influxUrl = "http://localhost:8086";
            string _dbUName = "admin";
            string _dbpwd = "admin";

            var providers =
                StorageProviderFabric.StorageProviders.Where(p => p.Key.ToLowerInvariant().Contains("influxdb"))
                    .ToList();

            Assert.AreEqual(true, providers.Count > 0);

            var influxDbProviderData = providers.First();

            // using anonymous types?
            var settings = new Dictionary<string, string>
            {
                ["NetworkPath"] = _influxUrl,
                ["UserName"] = _dbUName,
                ["Password"] = _dbpwd
            };

            // creating our IStorageProvider
            var provider = StorageProviderFabric.CreateStorageProvider(influxDbProviderData.Key, settings);

            Assert.AreEqual(true, provider != null);

            // Creating new Storage Context (it's the only and central connection to the underlying storage provider)
            var ctx = new StorageContext(provider);

            Assert.AreEqual(true, ctx.StorageProvider.IsConnected);

            // clearing the DB
            await ctx.StorageProvider.ClearAllStores();

            var model = await Model.GetOrCreateModel("All_Desktops", ctx);

            Assert.AreEqual(true, model.StorageContext.StorageProvider.IsConnected);

            var entity = new Entity
            {
                Id = Guid.NewGuid().ToString(),
                Title = "abcdef",
                TLBirth = DateTime.UtcNow,
                TLChange = DateTime.UtcNow,
                TLDeath = DateTime.MaxValue
            };

            await model.AddEntity(entity);

            Assert.AreEqual(1, model.Entities.Count);
        }
    }
}
