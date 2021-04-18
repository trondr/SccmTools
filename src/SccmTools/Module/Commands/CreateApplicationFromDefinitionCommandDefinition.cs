using Common.Logging;
using NCmdLiner.Attributes;
using SccmTools.Library.Infrastructure;
using SccmTools.Library.Module.Commands.CreateApplication;

namespace SccmTools.Module.Commands
{
    public class CreateApplicationFromDefinitionCommandDefinition: CommandDefinition
    {
        private readonly ICreateApplicationFromDefinitionCommandProvider _createApplicationFromDefinitionCommandProvider;
        private readonly ILog _logger;

        public CreateApplicationFromDefinitionCommandDefinition(ICreateApplicationFromDefinitionCommandProvider createApplicationFromDefinitionCommandProvider, ILog logger)
        {
            _createApplicationFromDefinitionCommandProvider = createApplicationFromDefinitionCommandProvider;
            _logger = logger;
        }

        [Command(
            Summary="Create a SCCM 2012 application from a custom package definition file.", 
            Description = "Create SCCM 2012 application from a package definition file. The PackageDefinition.sms format is documented here: https://technet.microsoft.com/en-ca/library/bb632631.aspx. This command is useful in simpler script install scenarios where only an install and an uninstall command line is suffcient. Application detection method will be based on the msi product code of the installed msi or on a specified registry value. The msi product code on the format '{{...guid...}}', should be defined in the custom section key [DetectionMethod]MsiProductCode. If the product code is not manually defined, the msi product code will be attempted automatically retrieved from any msi file found by directory search of the content folder, the folder where the package definition file is located. If more than one msi file is found, an exception is thrown asking for manual definition of product code. If the content directory does not contain a msi file, a registry value can be defined as detection method by specifying the value [DetectionMethod]RegistryValue. The install script is in this case responsible for writing this registry value on install and removing the registry value on uninstall. The package definition file is required to have a [INSTALL] program and a [UNINSTALL] program. The following values are recognized and read from the package definition file: [Package Definition]Name, [Package Definition]Version, [Package Definition]Publisher, [Package Definition]Language, [Package Definition]Comment, [INSTALL]CommandLine, [INSTALL]Icon, [UNINSTALL]CommandLine, [DetectionMethod]MsiProductCode, [DetectionMethod]RegistryValue, [DetectionMethod]RegistryValueIs64Bit. The location of the package definition file must be on a network drive or an UNC path. The command will automatically derive the UNC path from a network drive."
            )]
        public int CreateApplicationFromDefinition(
            [OptionalCommandParameter(Description = "Package definition file as specified by https://technet.microsoft.com/en-ca/library/bb632631.aspx. It is required that file path is a unc path or on a network drive. If this package definition file is not specified on the command line a file dialog will be shown to the user.", AlternativeName = "pf", ExampleValue = @"\\servername\appsource\Some Application 1.0\Pkg\PackageDefinition.sms", DefaultValue = "")]
            string packageDefinitionFile
            )
        {
            RunTimeRequirements.AssertAll();
            return _createApplicationFromDefinitionCommandProvider.CreateApplicationFromDefinition(packageDefinitionFile);
        }

    }
}
