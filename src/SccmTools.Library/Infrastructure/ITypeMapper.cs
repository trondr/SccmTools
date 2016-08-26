namespace SccmTools.Library.Infrastructure
{
    public interface ITypeMapper
    {
        T Map<T>(object source);
    }
}
