﻿using System;
using Microsoft.ConfigurationManagement.DesiredConfigurationManagement;
using NUnit.Framework;
using SccmTools.Infrastructure;
using SccmTools.Library.Module.Commands.CreateApplication2;
using SccmTools.Tests.UnitTests.Module.Common.IO;

namespace SccmTools.Tests.UnitTests.Module.Commands.CreateApplication2
{
    [TestFixture()]
    public class PackageDefinitionProviderTests
    {
        [Test()]
        public void WritePackageDefinitionTest()
        {
            var testPackageDefinition = GetTestPackageDefinition();
            using (var testPacageDefinitionFile = new TestIniFile(string.Empty))
            {
                using (var bootStrapper = new BootStrapper())
                {
                    var target = bootStrapper.Container.Resolve<IPackageDefinitionProvider>();
                    target.WritePackageDefinition(testPacageDefinitionFile.TestIniFileName, testPackageDefinition);
                    var actual = target.ReadPackageDefinition(testPacageDefinitionFile.TestIniFileName);
                    Assert.AreEqual("Example Name", actual.Name);
                }
            }
        }

        private PackageDefinition2 GetTestPackageDefinition()
        {
            var registryValue = new RegistryValue(RegistryRootKey.LocalMachine,@"Software\MyCompany\MyApplication","IsInstalled","1",true);

            var packageDefinition = new PackageDefinition2("Example Name", "1.0.0.0", "My Company", "A comment", "EN",
                "Install.cmd", "UnIntall.cmd", null, "{5852FC46-329F-4A34-B42F-48CFE0290BB1}", registryValue, true, null);
            return packageDefinition;
        }

        [Test]
        public void ReadPackageDefinitionTest()
        {
            string packageDefinitionTestString = GetPackageDefinitionTestString();
            using (var testPacageDefinitionFile = new TestIniFile(packageDefinitionTestString))
            {
                using (var bootStrapper = new BootStrapper())
                {
                    var target = bootStrapper.Container.Resolve<IPackageDefinitionProvider>();
                    var actual = target.ReadPackageDefinition(testPacageDefinitionFile.TestIniFileName);
                    Assert.AreEqual("_S_ProductName_S_", actual.Name);
                    Assert.AreEqual(@"[HKLM\Software\MyCompany\MyApplication]IsInstalled=1", actual.RegistryValue);
                    Assert.AreEqual(true,actual.RegistryValueIs64Bit);
                }
            }            
        }

       private string GetPackageDefinitionTestString()
        {
            var packageDefinitionTestString =
@"
[PDF]
Version = 2.0

[Package Definition]
Name = _S_ProductName_S_
Version = 1.0.16239.1
Publisher = _S_CompanyName_S_
Language = EN
Comment = Test Comment
Programs = INSTALL,UNINSTALL

[INSTALL]
Name = INSTALL
CommandLine = Install.cmd Install > ""%Public%\Logs\_S_ServiceScriptInstallProjectNameU_S_1_0_16239_1_Install.cmd.log""
CanRunWhen = AnyUserStatus
UserInputRequired = False
AdminRightsRequired = True
UseInstallAccount = True
Run = Minimized
Icon = App.ico
Comment = 

[UNINSTALL]
Name = UNINSTALL
CommandLine = Install.cmd UnInstall > ""%Public%\Logs\_S_ServiceScriptInstallProjectNameU_S_1_0_16239_1_UnInstall.cmd.log""
CanRunWhen = AnyUserStatus
UserInputRequired = False
AdminRightsRequired = True
UseInstallAccount = True
Run = Minimized
Comment = 

[DetectionMethod]
MsiProductCode = {5852FC46-329F-4A34-B42F-48CFE0290BBB}
RegistryValue = [HKLM\Software\MyCompany\MyApplication]IsInstalled=1
RegistryValueIs64Bit = true
";
            return packageDefinitionTestString;
        }


        private string GetPackageDefinitionTestStringWithFileDetectionMethodFormatError()
        {
            var packageDefinitionTestString =
@"
[PDF]
Version = 2.0

[Package Definition]
Name = _S_ProductName_S_
Version = 1.0.16239.1
Publisher = _S_CompanyName_S_
Language = EN
Comment = Test Comment
Programs = INSTALL,UNINSTALL

[INSTALL]
Name = INSTALL
CommandLine = Install.cmd Install > ""%Public%\Logs\_S_ServiceScriptInstallProjectNameU_S_1_0_16239_1_Install.cmd.log""
CanRunWhen = AnyUserStatus
UserInputRequired = False
AdminRightsRequired = True
UseInstallAccount = True
Run = Minimized
Icon = App.ico
Comment = 

[UNINSTALL]
Name = UNINSTALL
CommandLine = Install.cmd UnInstall > ""%Public%\Logs\_S_ServiceScriptInstallProjectNameU_S_1_0_16239_1_UnInstall.cmd.log""
CanRunWhen = AnyUserStatus
UserInputRequired = False
AdminRightsRequired = True
UseInstallAccount = True
Run = Minimized
Comment = 

[DetectionMethod]
MsiProductCode = {5852FC46-329F-4A34-B42F-48CFE0290BBB}
File1={""FileName"":""FileName1"",""FileVersion"":{""Major"":1,""Minor"":0,""Build"":0,""Revision"":0,""MajorRevision"":0,""MinorRevision"":0},""ModifyDateTime""""2017-01-08T13:48:53.2047827+01:00""}

";
            return packageDefinitionTestString;
        }
    }
}