using System;

namespace SccmTools.Library.Module.Commands.CreateApplication2
{
    public class InvalidRegistryRootKeyException : Exception
    {
        public InvalidRegistryRootKeyException()
        {
        }

        public InvalidRegistryRootKeyException(string message)
            : base(message)
        {
        }

        public InvalidRegistryRootKeyException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}