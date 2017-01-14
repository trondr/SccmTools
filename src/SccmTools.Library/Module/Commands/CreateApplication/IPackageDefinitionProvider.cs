namespace SccmTools.Library.Module.Commands.CreateApplication
{
    public interface IPackageDefinitionProvider
    {
        PackageDefinition ReadPackageDefinition(string packageDefinitionFileName);

        void WritePackageDefinition(string packageDefinitionFileName, PackageDefinition packageDefinition);
    }
}