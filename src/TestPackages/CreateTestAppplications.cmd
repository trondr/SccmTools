Set ApplicationsUncRootFolder=\\<cmserver>\pkgsrc\Applications

Set SccmToolsExe=%~dp0..\..\bin\Release\SccmTools\SccmTools.exe

"%SccmToolsExe%" CreateApplicationFromDefinition /packageDefinitionFile="%ApplicationsUncRootFolder%\Test Application Service 1\Script\PackageDefinition.sms"
"%SccmToolsExe%" CreateApplicationFromDefinition /packageDefinitionFile="%ApplicationsUncRootFolder%\Test Application Service 2\Script\PackageDefinition.sms"
"%SccmToolsExe%" CreateApplicationFromDefinition /packageDefinitionFile="%ApplicationsUncRootFolder%\Test Application with Depency\Script\PackageDefinition.sms"