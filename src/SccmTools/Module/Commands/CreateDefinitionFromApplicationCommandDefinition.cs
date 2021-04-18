using NCmdLiner;
using NCmdLiner.Attributes;
using SccmTools.Library.Infrastructure;
using SccmTools.Library.Module.Commands.CreateDefinitionFromApplication;

namespace SccmTools.Module.Commands
{
    public class CreateDefinitionFromApplicationCommandDefinition : CommandDefinition
    {
        private readonly ICreateDefinitionFromApplicationCommandProviderFactory _commandProviderFactory;

        public CreateDefinitionFromApplicationCommandDefinition(ICreateDefinitionFromApplicationCommandProviderFactory commandProviderFactory)
        {
            _commandProviderFactory = commandProviderFactory;
        }

        [Command(Description = "Create PackageDefinition.sms from existing Sccm application. The PackageDefinition.sms file will be stored in the content location folder of the application if the file does not allready exist.", Summary = "Create PackageDefinition.sms from existing Sccm application.")]
        public Result<int> CreateDefinitionFromApplication(
            [RequiredCommandParameter(Description = "Sccm application name", AlternativeName = "an",
                ExampleValue = "Citrix Receiver 4.3")]
            string applicationName,
            [RequiredCommandParameter(Description = "Sccm application version", AlternativeName = "av",
                ExampleValue = "4.3")]
            string applicationVersions)
        {
            RunTimeRequirements.AssertAll();
            var commandProvider = _commandProviderFactory.GetCreateDefinitionFromApplicationCommandProvider();
            var result = commandProvider.CreateDefinitionFromApplication(applicationName, applicationVersions);
            return result;
        }
    }
}
