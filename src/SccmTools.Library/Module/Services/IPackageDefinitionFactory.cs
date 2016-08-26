namespace SccmTools.Library.Module.Services
{
    public interface IPackageDefinitionFactory
    {
        IPackageDefinition GetPackageDefinition(string packageDefinitionFile);

        void Release(IPackageDefinition packageDefinition);
    }
}