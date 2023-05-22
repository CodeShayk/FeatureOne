namespace FeatureOne
{
    /// <summary>
    /// Interface to implement custom logger.
    /// </summary>
    public interface IFeatureLogger
    {
        /// <summary>
        /// Implement the debug log method
        /// </summary>
        /// <param name="message">log message</param>
        void Debug(string message);

        /// <summary>
        /// Implement the error log method
        /// </summary>
        /// <param name="message">log message</param>
        void Error(string message);

        /// <summary>
        /// Implement the info log method
        /// </summary>
        /// <param name="message">log message</param>
        void Info(string message);

        /// <summary>
        /// Implement the warn log method
        /// </summary>
        /// <param name="message">log message</param>
        void Warn(string message);
    }
}