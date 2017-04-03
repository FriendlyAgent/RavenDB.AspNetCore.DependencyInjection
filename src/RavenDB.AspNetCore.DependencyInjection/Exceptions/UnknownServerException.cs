using System;

namespace RavenDB.AspNetCore.DependencyInjection.Exceptions
{
    /// <summary>
    /// Represents errors that occur during application execution/>.
    /// </summary>
    public class UnknownServerException
        : Exception
    {
        /// <summary>
        /// Initializes a new instance of the UnknownServerException class.
        /// </summary>
        public UnknownServerException()
        : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the UnknownServerException class with a specified error
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public UnknownServerException(string message)
        : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the UnknownServerException class with a specified error
        /// </summary>
        /// <param name="format">The message that describes the error and contains the arguments, for example : Error: {0}.</param>
        /// <param name="args">Arguments used in the formatted message.</param>
        public UnknownServerException(string format, params object[] args)
        : base(string.Format(format, args))
        {
        }

        /// <summary>
        /// Initializes a new instance of the UnknownServerException class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference.</param>
        public UnknownServerException(string message, Exception innerException)
        : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the UnknownServerException class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="format">The message that describes the error and contains the arguments, for example : Error: {0}.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference.</param>
        /// <param name="args">Arguments used in the formatted message.</param>
        public UnknownServerException(string format, Exception innerException, params object[] args)
        : base(string.Format(format, args), innerException)
        {
        }
    }
}