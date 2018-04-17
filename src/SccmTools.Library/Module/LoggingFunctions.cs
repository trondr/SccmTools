using System;
using Common.Logging;
using LanguageExt;

namespace SccmTools.Library.Module
{
    public static class LoggingFunctions
    {
        private static readonly Func<Type, ILog> LogFactory = LogManager.GetLogger;
        private static readonly Func<Type, ILog> CachedLogFactory = LogFactory.Memo();
        public static ILog Logger(this object obj) => CachedLogFactory(obj.GetType());
        public static ILog Logger(Type type) => CachedLogFactory(type);
        public static ILog Logger<T>() => CachedLogFactory(typeof(T));
    }
}
