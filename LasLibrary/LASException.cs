using System;

namespace LASLibrary
{
    public enum LasError
    {
        LasNoneError,
        LasDebugError,
        LasWarningError,
        LasFailureError,
        LasFatalError
    };

    /// <summary>
    /// LASException class
    /// </summary>
    public class LasException : ApplicationException
    {
        readonly LasError _type;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="message">string to show</param>
        public LasException(string message) : base(message)
        {
            _type = LasError.LasWarningError;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="message">string to show</param>
        /// <param name="type">type of exception</param>
        public LasException(string message, LasError type) : base(message)
        {
            _type = type;
        }

        public override string ToString()
        {
            return string.Format("{0} : {1}", _type, Message);
        }
    }
}