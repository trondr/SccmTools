using System;

namespace SccmTools.Library.Module.Commands.CreateApplication
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