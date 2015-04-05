namespace SccmTools.Library.Common.Caching
{
    public interface ICacheValidator
    {
        bool IsValid(CacheValue cacheValue);
    }
}