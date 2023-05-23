namespace FeatureOne.Tests
{
    public class ConsoleLogger : IFeatureLogger
    {
        public void Debug(string message)
        {
            Console.WriteLine(message);
        }

        public void Error(string message, Exception e)
        {
            Console.WriteLine(message);
        }

        public void Info(string message)
        {
            Console.WriteLine(message);
        }

        public void Warn(string message)
        {
            Console.WriteLine(message);
        }
    }
}