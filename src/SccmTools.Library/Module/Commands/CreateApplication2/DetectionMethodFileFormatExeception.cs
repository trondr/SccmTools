using System;

namespace SccmTools.Library.Module.Commands.CreateApplication2
{
    public class DetectionMethodFileFormatExeception : Exception
    {
        public DetectionMethodFileFormatExeception()
        {
        }

        public DetectionMethodFileFormatExeception(string message)
            : base(message)
        {
        }

        public DetectionMethodFileFormatExeception(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}