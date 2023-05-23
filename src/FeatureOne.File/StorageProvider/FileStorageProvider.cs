using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Runtime.CompilerServices;
using FeatureOne.Cache;
using FeatureOne.Core;
using FeatureOne.Core.Stores;
using FeatureOne.Json;

[assembly: InternalsVisibleTo("FeatureOne.File.Tests")]

namespace FeatureOne.File.StorageProvider
{
    internal class FileStorageProvider : IStorageProvider
    {
        internal readonly FileConfiguration configuration;
        internal readonly IFileReader reader;
        internal readonly IToggleDeserializer deserializer;
        internal readonly ICache cache;
        internal const string cacheKey = "file_state";

        public FileStorageProvider(FileConfiguration configuration, ICache cache = null, IConditionDeserializer conditionDeserializer = null)
        {
            this.configuration = configuration ?? new FileConfiguration();
            this.reader = new FileReader(configuration);
            this.deserializer = new ToggleDeserializer(conditionDeserializer ?? new ConditionDeserializer());
            this.cache = cache ?? new FeatureCache();
        }

        public FileStorageProvider(FileConfiguration configuration, IFileReader reader, IToggleDeserializer deserializer, ICache cache)
        {
            this.reader = reader ?? new FileReader(configuration);
            this.deserializer = deserializer ?? new ToggleDeserializer();
            this.cache = cache ?? new FeatureCache();
            this.configuration = configuration ?? new FileConfiguration();
        }

        public IFeature[] GetByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            FileState fileState = null;

            if (configuration.CacheSettings?.EnableCache ?? false)
                fileState = (FileState)cache.Get(cacheKey);

            if (fileState == null)
                fileState = reader.Read();

            if (configuration.CacheSettings?.EnableCache ?? false)
            {
                var policy = configuration.CacheSettings.Expiry.GetPolicy();

                //Be aware that FileSystemWatcher.EnableRaisingEvents, that is used behind HostFileChangeMonitor, captures the ExecutionContext
                //Due to this HostFileChangeMonitor instance will not be garbage-collected and will live much longer
                //https://github.com/dotnet/runtime/issues/31100
                //https://github.com/dotnet/runtime/issues/30954
                //https://github.com/dotnet/corefx/pull/41872
                var monitor = new HostFileChangeMonitor(new List<string> { configuration.FilePath });
                policy.ChangeMonitors.Add(monitor);

                cache.Add(cacheKey, fileState, policy);
            }

            return fileState?.Records != null && fileState.Records.Any()
              ? fileState.Records.Where(x => !string.IsNullOrEmpty(x.Toggle)).Select(f => new Feature(f.Name, deserializer.Deserialize(f.Toggle))).ToArray()
              : Array.Empty<IFeature>();
        }
    }
}