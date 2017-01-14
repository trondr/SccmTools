using System;

namespace SccmTools.Library.Module.Commands.CreateApplication
{
    public class InvalidRegistryValueFormatException : Exception
    {
        public InvalidRegistryValueFormatException()
        {
        }

        public InvalidRegistryValueFormatException(string message)
            : base(message)
        {
        }

        public InvalidRegistryValueFormatException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}