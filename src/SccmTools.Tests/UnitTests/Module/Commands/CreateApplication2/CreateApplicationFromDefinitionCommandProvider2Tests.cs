using System.IO;
using NUnit.Framework;
using SccmTools.Infrastructure;
using SccmTools.Library.Module.Commands.CreateApplication;

namespace SccmTools.Tests.UnitTests.Module.Commands.CreateApplication2
{
    [TestFixture(Category = "IntegrationTests")]
    public class CreateApplicationFromDefinitionCommandProvider2Tests
    {
        [Test(Description = @"This intergration test requires that the package definition file exists on the path: '\\sccmserver1.test.local\pkgsrc\Applications\TestApplication\Script\PackageDefinition.sms'. If your sccm 2012 server is namned differently change the path accordingly. Also it is required that the user running the test is minimum appplication author on the SCCM server.")]
        public void CreateApplicationFromDefinitionTest()
        {
            var packageDefinitionFileName = Path.Combine(@"\\sccmserver1.test.local\pkgsrc\Applications\Test Application\Script\PackageDefinition.sms");
            CreateTestApplicationContent(packageDefinitionFileName);
            using (var bootStrapper = new BootStrapper())
            {
                var target = bootStrapper.Container.Resolve<ICreateApplicationFromDefinitionCommandProvider>();
                target.CreateApplicationFromDefinition(packageDefinitionFileName);
            }
        }

        private void CreateTestApplicationContent(string packageDefinitionFileName)
        {
            var packageDefinitionFile = new FileInfo(packageDefinitionFileName);
            if (!Directory.Exists(packageDefinitionFile.Directory.FullName))
            {
                Directory.CreateDirectory(packageDefinitionFile.Directory.FullName);
            }
            var packageDefinitionString = GetPackageDefinitionTestStringWithRegistryValue();
            using (var sw = new StreamWriter(packageDefinitionFile.FullName))
            {
                sw.Write(packageDefinitionString);
            }
            var installCmd = Path.Combine(packageDefinitionFile.Directory.FullName, "Install.cmd");
            using (var sw = new StreamWriter(installCmd))
            {
                sw.WriteLine("reg add \"HKLM\\Software\\MyCompany\\Test Application\" /v \"IsInstalled\" /t \"REG_SZ\" /d \"yes\"");
            }

            var unInstallCmd = Path.Combine(packageDefinitionFile.Directory.FullName, "UnInstall.cmd");
            using (var sw = new StreamWriter(unInstallCmd))
            {
                sw.WriteLine("reg delete \"HKLM\\Software\\MyCompany\\Test Application\" /v \"IsInstalled\" /f");
            }
        }

        private string GetPackageDefinitionTestStringWithRegistryValue()
        {
            var packageDefinitionTestString =
@"
[PDF]
Version = 2.0

[Package Definition]
Name = Test Application
Version = 1.0.16239.1
Publisher = MyCompany
Language = EN
Comment = Test Comment
Programs = INSTALL,UNINSTALL

[INSTALL]
Name = INSTALL
CommandLine = Install.cmd > ""%Public%\Logs\Test_Application_1_0_16239_1_Install.cmd.log""
CanRunWhen = AnyUserStatus
UserInputRequired = False
AdminRightsRequired = True
UseInstallAccount = True
Run = Minimized
Icon = App.ico
Comment = 

[UNINSTALL]
Name = UNINSTALL
CommandLine = UnInstall.cmd > ""%Public%\Logs\Test_Application_1_0_16239_1_UnInstall.cmd.log""
CanRunWhen = AnyUserStatus
UserInputRequired = False
AdminRightsRequired = True
UseInstallAccount = True
Run = Minimized
Comment = 

[DetectionMethod]
MsiProductCode = 
RegistryValue = [HKLM\Software\MyCompany\Test Application]IsInstalled=yes
RegistryValueIs64Bit = true
";
            return packageDefinitionTestString;
        }
    }
}