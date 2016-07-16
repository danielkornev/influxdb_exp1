using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using ZU.Core.Developer;
using ZU.Framework;

namespace ZU.Storage
{
    public static class StorageProviderFabric
    {
        private static readonly Dictionary<string, Type> storageProviders = new Dictionary<string, Type>();

        public static ImmutableDictionary<string, Type> StorageProviders => storageProviders.ToImmutableDictionary();
        

        public static void RegisterIStorageProvider(string providerName, Type providerType)
        {
            if (storageProviders.ContainsKey(providerName)) throw new Exception("Storage Provider \"" + providerName + "\" is already registered.");

            storageProviders.Add(providerName, providerType);
        }


        public static IStorageProvider CreateStorageProvider(string providerName, Dictionary<string, string> settings)
        {
            if (!storageProviders.ContainsKey(providerName))
                throw new Exception("Storage Provider \"" + providerName + "\" is not registered.");

            var type = storageProviders[providerName];

            try
            {
                var provider = Activator.CreateInstance(type);

                var result = provider as IStorageProvider;

                if (result == null)
                {
                    throw new Exception("The provided Provider class doesn't implement IStorageProvider.");
                }

                // Instantiating
                result.Instantiate(settings);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create a Storage Provider \"" + providerName + "\" because of:\n" + ex.Message );
            }
        }
    }
}