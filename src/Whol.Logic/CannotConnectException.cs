using System;
using System.Runtime.Serialization;

namespace Whol.Logic
{
    [Serializable]
    public class CannotConnectException : Exception
    {
        public CannotConnectException()
        {
        }

        public CannotConnectException(string message) : base(message)
        {
        }

        public CannotConnectException(string message, Exception inner) : base(message, inner)
        {
        }

        protected CannotConnectException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
