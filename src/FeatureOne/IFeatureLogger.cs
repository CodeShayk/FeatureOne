namespace FeatureOne
{
    public interface IFeatureLogger
    {
        void Debug(string message);

        void Error(string message);

        void Info(string message);

        void Warn(string message);
    }
}