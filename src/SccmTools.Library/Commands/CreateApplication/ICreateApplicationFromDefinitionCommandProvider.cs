namespace SccmTools.Library.Commands.CreateApplication
{
    public interface ICreateApplicationFromDefinitionCommandProvider
    {
        int CreateApplicationFromDefinition(string packageDefinitionFile);
    }
}
