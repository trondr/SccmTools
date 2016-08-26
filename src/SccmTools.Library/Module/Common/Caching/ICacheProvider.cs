namespace SccmTools.Library.Common.Caching
{
    public interface ICacheProvider
    {
        object Get(string cacheKey);
        void Put(string cacheKey, object item);
    }
}
