using FeatureOne.Cache;

namespace FeatureOne.File
{
    public class FileConfiguration
    {
        public FileConfiguration()
        {
            CacheSettings = new CacheSettings();
        }

        public string FilePath { get; set; }
        public CacheSettings CacheSettings { get; set; }
    }
}