using System;
using Common.Logging;
using NCmdLiner.Attributes;
using SccmTools.Library.Commands.CreateApplication;
using SccmTools.Library.Infrastructure;

namespace SccmTools.Commands
{
    public class CreateApplicationFromPackageDefintionCommandDefinition: CommandDefinition
    {
        private readonly ICreateApplicationFromPackageDefinitionCommandProvider _createApplicationFromPackageDefinitionCommandProvider;
        private readonly ILog _logger;

        public CreateApplicationFromPackageDefintionCommandDefinition(ICreateApplicationFromPackageDefinitionCommandProvider createApplicationFromPackageDefinitionCommandProvider, ILog logger)
        {
            _createApplicationFromPackageDefinitionCommandProvider = createApplicationFromPackageDefinitionCommandProvider;
            _logger = logger;
        }

        [Command(Description = "Create a SCCM 2012 application from a package definition file (PackageDefinition.sms). This is useful in simpler script install scenarios where only an install and uninstall command line is required and the application is based on a msi file. The package definition file is required to have a [INSTALL] program and a [UNINSTALL] program. The package defintion file is required to be based on a msi package from which the msi product code can be used in application detection method. The msi product code on the format '{{...guid...}}' is required to be to be located any where in the [Package Definition]Comment field. The following values are recognized and read from the package definition file: [Package Definition]Name, [Package Definition]Version, [Package Definition]Publisher, [Package Definition]Language, [Package Definition]Comment=<msi product code must be provided somewhere in this comment field>, [INSTALL]CommandLine, [INSTALL]Icon, [UNINSTALL]CommandLine."
            )]
        public int CreateApplicationFromPackageDefinition(
            [OptionalCommandParameter(Description = "Package definition file as specified by https://technet.microsoft.com/en-ca/library/bb632631.aspx. It is required that file path is a unc path. If this parameter is not specified a file dialog will be shown to the user.", AlternativeName = "pf", ExampleValue = @"\\servername\appsource\Some Application 1.0\Pkg\PackageDefinition.sms", DefaultValue = "")]
            string packageDefinitionFile
            )
        {            
            return _createApplicationFromPackageDefinitionCommandProvider.CreateApplicationFromPackageDefinition(packageDefinitionFile);
        }
    }
}
