using System;
using System.Runtime.Serialization;

namespace GPIOLibrary.Models
{
    public class GpioPinInitializeException : Exception
    {
        public GpioPinInitializeException() { }
        public GpioPinInitializeException(string message) : base(message) { }
        public GpioPinInitializeException(string message, Exception innerException) : base(message, innerException) { }
        protected GpioPinInitializeException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
