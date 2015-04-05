//Source: http://blog.willbeattie.net/2010/08/caching-as-cross-cutting-concern-using.html

using System.Linq;
using Castle.DynamicProxy;
using Common.Logging;
using SccmTools.Library.Common.Caching;

namespace SccmTools.Infrastructure
{
    public class CacheAspect : IInterceptor
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly ILog _logger;

        public CacheAspect(ICacheProvider cacheProvider, ILog logger)
        {
            _cacheProvider = cacheProvider;
            _logger = logger;
        }

        public void Intercept(IInvocation invocation)
        {
            if (invocation.Method.ReflectedType == typeof (void))
            {
                if (_logger.IsTraceEnabled) _logger.TraceFormat("Method '{0}' has no return value, nothing to cache.", invocation.Method.Name);
                invocation.Proceed();
            }
            var cacheKey = BuildCacheKey(invocation);
            var cachedItem = _cacheProvider.Get(cacheKey);
            if (cachedItem != null)
            {
                if (_logger.IsTraceEnabled) _logger.TraceFormat("Item was chaced return the cached value for cachekey '{0}'", cacheKey);
                invocation.ReturnValue = cachedItem;
            }
            else
            {
                if (_logger.IsTraceEnabled) _logger.TraceFormat("Item was not cached (or expired) call the real thing and add return value to the cache under cachekey '{0}'." ,cacheKey);
                invocation.Proceed();
                if (invocation.ReturnValue != null)
                {
                    _cacheProvider.Put(cacheKey, invocation.ReturnValue);
                }
            }
        }

        private static string BuildCacheKey(IInvocation invocation)
        {            
            var arguments = (from arg in invocation.Arguments select arg.ToString()).ToArray();
            var argumentString = string.Join(",", arguments);
            var cacheKey = invocation.Method.Name + "(" + argumentString + ")";
            return cacheKey;
        }
    }
}