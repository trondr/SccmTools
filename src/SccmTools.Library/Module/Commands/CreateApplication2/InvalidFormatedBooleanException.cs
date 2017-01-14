using System;

namespace SccmTools.Library.Module.Commands.CreateApplication2
{
    public class InvalidFormatedBooleanException : Exception
    {
        public InvalidFormatedBooleanException()
        {
        }

        public InvalidFormatedBooleanException(string message)
            : base(message)
        {
        }

        public InvalidFormatedBooleanException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}