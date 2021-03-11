using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Digimarc.NTiff
{
    public class TiffWriteException : Exception
    {
        public TiffWriteException() { }

        public TiffWriteException(string message) : base(message) { }

        public TiffWriteException(string message, Exception innerException) : base(message, innerException) { }

        protected TiffWriteException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
