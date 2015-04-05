using System;
using System.Collections.Generic;
using Common.Logging;

namespace SccmTools.Infrastructure
{
    public class LogFactory : ILogFactory
    {   
        private Dictionary<Type, ILog> LoggersDictionary
        {
            get { return _loggersDictionary ?? (_loggersDictionary = new Dictionary<Type, ILog>()); }
        }
        private Dictionary<Type, ILog> _loggersDictionary;
        
        public ILog GetLogger(Type type)
        {
            if (!LoggersDictionary.ContainsKey(type))
            {
                LoggersDictionary.Add(type, LogManager.GetLogger(type));
            }
            return LoggersDictionary[type];
        }
    }
}