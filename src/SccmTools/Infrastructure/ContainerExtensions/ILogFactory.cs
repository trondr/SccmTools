using System;
using Common.Logging;

namespace SccmTools.Infrastructure.ContainerExtensions
{
    public interface ILogFactory
    {
        ILog GetLogger(Type type);
    }
}