using System;
using Common.Logging;

namespace SccmTools.Infrastructure
{
    public interface ILogFactory
    {
        ILog GetLogger(Type type);
    }
}