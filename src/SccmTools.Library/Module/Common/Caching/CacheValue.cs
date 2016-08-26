using System;

namespace SccmTools.Library.Module.Common.Caching
{
    public class CacheValue
    {
        public object Value { get; set; }
        public DateTime Created { get; set; }
        public DateTime Visited { get; set; }
    }
}