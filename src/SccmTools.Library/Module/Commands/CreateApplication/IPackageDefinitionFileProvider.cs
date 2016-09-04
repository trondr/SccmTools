namespace SccmTools.Library.Module.Commands.CreateApplication
{
    public interface IPackageDefinitionFileProvider
    {
        File GetPackageDefinitionFile(string packageDefinitionFileName);
    }
}