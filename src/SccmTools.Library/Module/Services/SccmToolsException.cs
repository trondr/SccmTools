using System;
using System.Runtime.Serialization;

namespace SccmTools.Library.Module.Services
{
    public class SccmToolsException : Exception
    {
        public SccmToolsException(string message)
            : base(message)
        {
            
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public SccmToolsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected SccmToolsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}