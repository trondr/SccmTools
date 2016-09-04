namespace SccmTools.Library.Module.Commands.CreateApplication2
{
    public interface IPackageDefinitionFactory2
    {
        IPackageDefinition2 GetPackageDefinition(string packageDefinitionFile);

        void Release(IPackageDefinition2 packageDefinition);
    }
}