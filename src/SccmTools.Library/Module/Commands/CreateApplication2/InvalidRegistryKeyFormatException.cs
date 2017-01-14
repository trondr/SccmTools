using System;

namespace SccmTools.Library.Module.Commands.CreateApplication2
{
    public class InvalidRegistryKeyFormatException : Exception
    {
        public InvalidRegistryKeyFormatException()
        {
        }

        public InvalidRegistryKeyFormatException(string message)
            : base(message)
        {
        }

        public InvalidRegistryKeyFormatException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}