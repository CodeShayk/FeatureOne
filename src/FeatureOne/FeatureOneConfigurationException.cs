using System;

namespace FeatureOne
{
    /// <summary>
    /// Thrown when FeatureOne is misconfigured — for example when a toggle references a condition type
    /// that has not been registered, or when a custom condition type cannot be constructed.
    /// </summary>
    public class FeatureOneConfigurationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of <see cref="FeatureOneConfigurationException"/>.
        /// </summary>
        public FeatureOneConfigurationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="FeatureOneConfigurationException"/> with a message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public FeatureOneConfigurationException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="FeatureOneConfigurationException"/> with a message and inner exception.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The underlying cause.</param>
        public FeatureOneConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
