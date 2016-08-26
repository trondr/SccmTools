namespace SccmTools.Library.Module.Common.Caching
{
    public interface ICacheValidator
    {
        bool IsValid(CacheValue cacheValue);
    }
}