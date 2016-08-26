using System.Collections.Concurrent;
using Common.Logging;
using SccmTools.Library.Infrastructure;
using SccmTools.Library.Infrastructure.LifeStyles;

namespace SccmTools.Library.Common.Caching
{
    [Singleton]
    public class CacheProvider : ICacheProvider
    {
        private readonly ICacheValidator _cacheValidator;
        private readonly ICurrentDateTimeService _currentDateTimeService;
        private readonly ILog _logger;

        public CacheProvider(ICacheValidator cacheValidator, ICurrentDateTimeService currentDateTimeService, ILog logger)
        {
            _cacheValidator = cacheValidator;
            _currentDateTimeService = currentDateTimeService;
            _logger = logger;
        }

        private readonly object _synch = new object();

        private ConcurrentDictionary<string, CacheValue> Cache 
        {
            get
            {
                if (_cache != null) return _cache;
                lock (_synch)
                {
                    if (_cache == null)
                    {
                        _cache = new ConcurrentDictionary<string, CacheValue>();
                    }
                }
                return _cache;
            }
        }
        private volatile ConcurrentDictionary<string, CacheValue> _cache;

        public object Get(string cacheKey)
        {
            if (Cache.ContainsKey(cacheKey))
            {
                var cacheValue = Cache[cacheKey];
                if (_cacheValidator.IsValid(cacheValue))
                {
                    if (_logger.IsDebugEnabled) _logger.DebugFormat("Cache key '{0}' was found in cache. Return value: {1}", cacheKey, cacheValue.Value.ToString());
                    cacheValue.Visited = _currentDateTimeService.GetCurrentDateTimeUtc();
                    return cacheValue.Value;
                }
                if (_logger.IsDebugEnabled) _logger.DebugFormat("Cache key '{0}' has expired, remove from cache." , cacheKey);
                CacheValue cachevalue;
                Cache.TryRemove(cacheKey, out cachevalue);
            }
            //Cache key is not in cache
            if (_logger.IsDebugEnabled) _logger.DebugFormat("Cache key '{0}' is not in cache.", cacheKey);
            return null;
        }

        public void Put(string cacheKey, object item)
        {
            var newCacheValue = new CacheValue()
            {
                Value = item,
                Created = _currentDateTimeService.GetCurrentDateTimeUtc(),
                Visited = _currentDateTimeService.GetCurrentDateTimeUtc()
            };
            if (_logger.IsDebugEnabled) _logger.DebugFormat("Adding or updating to cache: '{0}' = {1}", cacheKey, newCacheValue.Value.ToString());
            Cache.AddOrUpdate(cacheKey, newCacheValue, (key, existingCacheValue) =>
            {
                // If this delegate is invoked, then the cache key already exists. 
                existingCacheValue.Value = newCacheValue.Value;
                existingCacheValue.Created = newCacheValue.Created;
                existingCacheValue.Visited = newCacheValue.Visited;
                return existingCacheValue;
            });
        }
    }
}