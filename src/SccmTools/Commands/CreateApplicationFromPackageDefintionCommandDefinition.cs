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

        [Command(Description = "Create a SCCM 2012 application from a leagacy package definition file. The package definition file is required to have a [INSTALL] program and a [UNINSTALL] program. The package defintion file is required to contain a msi package from which the msi product code can be used in application detection method. The msi product code on the format '{{...guid...}}' is required to be to be located any where in the [Package Definition]Comment field."
            )]
        public int CreateApplicationFromPackageDefinition(
            [RequiredCommandParameter(Description = "Package definition file. Ref https://technet.microsoft.com/en-ca/library/bb632631.aspx. Example:\n\n" +
                
                
            "[PDF]\n" +
            "Version=2.0\n" +
            "\n" +
            "[Package Definition]\n" +
            "Name=Some Appplication\n" +
            "Version=1.0.15092.1 - 1.0\n" +
            "Publisher=Some Company\n" +
            "Language=EN\n" +
            "Comment=Some comment. Msi product code anywhere in the comment {{288164B1-EF1F-4113-93D8-8FA2E09D9E34}} will be picked up by the CreateApplicationFromPackageDefinition command.\n" +
            "Programs=INSTALL,UNINSTALL\n" +
            "\n" +
            "[INSTALL]\n" +
            "Name=INSTALL\n" +
            "CommandLine=Install.cmd Install > \"%%Public%%\\InstallLogs\\Some_Application_1_0_15092_1_Install.cmd.log\"\n" +
            "CanRunWhen=AnyUserStatus\n" +
            "UserInputRequired=False\n" +
            "AdminRightsRequired=True\n" +
            "UseInstallAccount=True\n" +
            "Run=Minimized\n" +
            "\n" +
            "[UNINSTALL]\n" +
            "Name=REMOVE\n" +
            "CommandLine=Install.cmd UnInstall > \"%%Public%%\\InstallLogs\\Some_Application_1_0_15092_1_UnInstall.cmd.log\"\n" +
            "CanRunWhen=AnyUserStatus\n" +
            "UserInputRequired=False\n" +
            "AdminRightsRequired=True\n" +
            "UseInstallAccount=True\n" +
            "Run=Minimized\n"
                
                , AlternativeName = "pf", ExampleValue = @"c:\PackagePool\Some Application 1.0\Pkg\PackageDefinition.sms")]
            string packageDefinitionFile
            )
        {            
            return _createApplicationFromPackageDefinitionCommandProvider.CreateApplicationFromPackageDefinition(packageDefinitionFile);
        }
    }
}
