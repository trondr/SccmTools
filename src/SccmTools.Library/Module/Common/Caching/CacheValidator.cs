namespace SccmTools.Library.Module.Common.Caching
{
    public class CacheValidator : ICacheValidator
    {
        private readonly ICurrentDateTimeService _currentDateTimeService;

        public CacheValidator(ICurrentDateTimeService currentDateTimeService)
        {
            _currentDateTimeService = currentDateTimeService;
        }

        public bool IsValid(CacheValue cacheValue)
        {
            var now = _currentDateTimeService.GetCurrentDateTimeUtc();            
            var visitedAge = now - cacheValue.Visited;
            if (visitedAge.TotalSeconds < 60)
            {
                return true;
            }
            return false;
        }
    }
}