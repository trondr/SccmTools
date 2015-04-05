# SccmTools
=========================
SccmTools provides various commands for SCCM 2012 interaction

## Minimum Build Requirements
=========================
* MSBuild (http://www.microsoft.com/en-us/download/details.aspx?id=40760)
* Windows SDK (http://msdn.microsoft.com/en-us/windows/desktop/bg162891.aspx)
* .NET Framework 4.5.2 Runtime (http://go.microsoft.com/fwlink/?LinkId=397674)
* .NET Framework 4.5.2 Developer Pack (http://go.microsoft.com/fwlink/?LinkId=328857)
* .NET Framework 2.0/3.5 (Install from Windows Features on Windows 8.1/Windows Server 2012 R2)
* Wix Toolset 3.8 (http://wix.codeplex.com/downloads/get/762937)
* System Center 2012 R2 Configuration Manager Console
* The directory '.\SccmTools\bin\Libs' in the solution directory must contain:
	AdminUI.WqlQueryEngine.dll
	DcmObjectModel.dll
	Microsoft.ConfigurationManagement.ApplicationManagement.dll
	Microsoft.ConfigurationManagement.ApplicationManagement.MsiInstaller.dll
	Microsoft.ConfigurationManagement.ManagementProvider.dll
	
## Commands
=========================	
### CreateApplicationFromPackageDefinition

Create a SCCM 2012 application from a package definition file 
as documented here: https://technet.microsoft.com/en-ca/library/bb632631.aspx). 

The package definition file is required to have a [INSTALL] program and a [UNINSTALL] program.

The MSI product code on the format '{guid}' must be located any where in the '[Package Definition]Comment' field.
	
#### Supported package definition values:
	
	*[Package Definition]Name
	*[Package Definition]Version
	*[Package Definition]Publisher
	*[Package Definition]Language
	*[Package Definition]Comment=<msi product code must be provided somewhere in this comment field>
	*[INSTALL]CommandLine
	*[INSTALL]Icon
	*[UNINSTALL]CommandLine
		
	All other values in package definition file is ignored.
	
#### Example package definition file

```dosini
[PDF]
Version=2.0

[Package Definition]
Name=NCmdLiner Solution Creator
Version=1.0.15092.30
Publisher=github-com-trondr
Language=EN
Comment=NCmdLiner Solution Creator installs solution creator command into Windows Explorer context menu. The command creates a starting point solution.  Product code: {1D3BF4CD-E8F1-482C-9B86-5DEE24CFF8EB}
Programs=INSTALL,UNINSTALL

[INSTALL]
Name=INSTALL
CommandLine=Install.cmd Install > "%Public%\InstallLogs\NCmdLiner_Solution_Creator_1_0_15092_30_Install.cmd.log"
CanRunWhen=AnyUserStatus
UserInputRequired=False
AdminRightsRequired=True
UseInstallAccount=True
Run=Minimized
Icon=NCmdLiner.png

[UNINSTALL]
Name=UNINSTALL
CommandLine=Install.cmd UnInstall > "%Public%\InstallLogs\NCmdLiner_Solution_Creator_1_0_15092_30_UnInstall.cmd.log"
CanRunWhen=AnyUserStatus
UserInputRequired=False
AdminRightsRequired=True
UseInstallAccount=True
Run=Minimized
```