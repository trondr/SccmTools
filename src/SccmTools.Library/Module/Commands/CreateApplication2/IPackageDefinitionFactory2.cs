namespace SccmTools.Library.Module.Commands.CreateApplication2
{
    public interface IPackageDefinitionFactory2
    {
        PackageDefinition2 GetPackageDefinition(string packageDefinitionFile);

        void Release(PackageDefinition2 packageDefinition);
    }
}