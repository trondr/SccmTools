namespace SccmTools.Library.Commands.CreateApplication
{
    public interface ICreateApplicationFromPackageDefinitionCommandProvider
    {
        int CreateApplicationFromPackageDefinition(string packageDefinitionFile);
    }
}
