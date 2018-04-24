

using NCmdLiner;

namespace SccmTools.Library.Module.Commands.CreateDefinitionFromApplication
{
    public interface ICreateDefinitionFromApplicationCommandProvider
    {
        Result<int> CreateDefinitionFromApplication(string applicationName, string applicationVersion);
    }
}
