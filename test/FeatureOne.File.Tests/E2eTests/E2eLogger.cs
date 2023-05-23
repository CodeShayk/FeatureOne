namespace FeatureOne.File.Tests.E2eTests
{
    internal class E2eLogger : IFeatureLogger
    {
        public void Debug(string message) => Console.WriteLine("Debug:" + message);

        public void Error(string message, Exception e) => Console.WriteLine("Error:" + message + e.ToString());

        public void Info(string message) => Console.WriteLine("Info:" + message);

        public void Warn(string message) => Console.WriteLine("Warn:" + message);
    }
}