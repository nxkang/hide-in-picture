using System;
using System.Runtime.Serialization;

namespace InfoHidden.Service
{
    [Serializable]
    internal class PasswordErrorException : Exception
    {
        public PasswordErrorException()
        {
        }

        public PasswordErrorException(string message) : base(message)
        {
        }

        public PasswordErrorException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PasswordErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}