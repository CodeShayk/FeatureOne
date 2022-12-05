namespace FeatureOn.Logger
{
    public class NullLogger : IFeatureLogger
    {
        public void Info(string message) { }
        public void Debug(string message) { }
        public void Warn(string message) { }
        public void Error(string message) { }
    }
}