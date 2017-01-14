﻿using System;

namespace SccmTools.Library.Module.Commands.CreateApplication2
{
    public class DetectionMethodNotSpecifiedException : Exception
    {
        public DetectionMethodNotSpecifiedException()
        {
        }

        public DetectionMethodNotSpecifiedException(string message)
            : base(message)
        {
        }

        public DetectionMethodNotSpecifiedException(string message, Exception inner)
            : base(message, inner)
        {
        }

    }
}