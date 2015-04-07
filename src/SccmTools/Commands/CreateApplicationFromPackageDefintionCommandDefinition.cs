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

        [Command(Description = "Create a SCCM 2012 application from a package definition file (PackageDefinition.sms as documented here: https://technet.microsoft.com/en-ca/library/bb632631.aspx). This is useful in simpler script install scenarios where only install and uninstall command line is necessary and the application is installed by a msi file. Application detection method will be based on the msi product code. The product code, on the format '{{...guid...}}', can be manually defined anywhere in the [Package Definition]Comment value. If the product code is not manually defined the msi product code will be attempted automatically retrieved from the msi file found by directory search of the content folder (the folder where the package definition file is located). If more than one msi file is found, an exception is thrown asking for manual definition of product code. The package definition file is required to have a [INSTALL] program and a [UNINSTALL] program. The following values are recognized and read from the package definition file: [Package Definition]Name, [Package Definition]Version, [Package Definition]Publisher, [Package Definition]Language, [Package Definition]Comment=<msi product code can be provided somewhere in this comment field>, [INSTALL]CommandLine, [INSTALL]Icon, [UNINSTALL]CommandLine."
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
