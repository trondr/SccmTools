namespace SccmTools.Library.Services
{
    public interface IPackageDefinitionFactory
    {
        IPackageDefinition GetPackageDefinition(string packageDefinitionFile);

        void Release(IPackageDefinition packageDefinition);
    }
}