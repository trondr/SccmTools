namespace SccmTools.Library.Module.Commands.CreateApplication
{
    public interface ICreateApplicationFromDefinitionCommandProvider
    {
        int CreateApplicationFromDefinition(string packageDefinitionFileName);
    }
}