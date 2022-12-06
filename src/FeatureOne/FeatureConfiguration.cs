namespace FeatureOne
{
    public class FeatureConfiguration
    {
        public bool UseCache { get; set; } = true;
        public TimeSpan SlidingExpiry { get; set; } = TimeSpan.FromMinutes(5);
        public IFeatureLogger Logger { get; set; } = new NullLogger();
    }
}