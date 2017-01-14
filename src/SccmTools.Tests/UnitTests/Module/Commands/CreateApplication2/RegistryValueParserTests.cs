using Microsoft.ConfigurationManagement.DesiredConfigurationManagement;
using NUnit.Framework;
using SccmTools.Library.Module.Commands.CreateApplication2;

namespace SccmTools.Tests.UnitTests.Module.Commands.CreateApplication2
{
    [TestFixture()]
    public class RegistryValueParserTests
    {
        [Test()]
        [TestCase(@"[HKLM\Software\MyCompany]IsInstalled=true", "true", RegistryRootKey.LocalMachine, @"Software\MyCompany", "IsInstalled", "true", true)]
        [TestCase(@"[HKLM\Software\MyCompany]IsInstalled=false", "true", RegistryRootKey.LocalMachine, @"Software\MyCompany", "IsInstalled", "false", true)]
        [TestCase(@"[HKLM\Software\MyCompany]IsInstalled=true", "false", RegistryRootKey.LocalMachine, @"Software\MyCompany", "IsInstalled", "true", false)]
        [TestCase(@"[HKLM\Software\MyCompany]IsInstalled=false", "false", RegistryRootKey.LocalMachine, @"Software\MyCompany", "IsInstalled", "false", false)]
        public void ParseRegistryValueTest(string registryValueString, string isRegistryValue64BitString, RegistryRootKey registryRootKey, string key, string valueName, string value, bool isRegistryValue64Bit)
        {
            var target = new RegistryValueParser();
            var actual = target.ParseRegistryValue(registryValueString, isRegistryValue64BitString);
            Assert.AreEqual(registryRootKey,actual.RootKey,"RootKey");
            Assert.AreEqual(key,actual.Key,"Key");
            Assert.AreEqual(valueName, actual.ValueName, "ValueName");
            Assert.AreEqual(value, actual.Value, "Value");
            Assert.AreEqual(isRegistryValue64Bit, actual.Is64Bit,"Is64Bit");            
        }

        [Test()]
        [TestCase(@"[HKLS\Software\MyCompany]IsInstalled=false", "false", RegistryRootKey.LocalMachine, @"Software\MyCompany", "IsInstalled", "false", false)]
        public void ParseRegistryValueTest_InvalidRootKey_Throw_InvalidRegistryKeyFormatException(string registryValueString, string isRegistryValue64BitString, RegistryRootKey registryRootKey, string key, string valueName, string value, bool isRegistryValue64Bit)
        {
            var target = new RegistryValueParser();
            Assert.Throws<InvalidRegistryKeyFormatException>(() =>
            {
                var actual = target.ParseRegistryValue(registryValueString, isRegistryValue64BitString);
            });
        }
        [Test()]
        [TestCase(@"[HKLM\Software\MyCompany]=false", "false", RegistryRootKey.LocalMachine, @"Software\MyCompany", "IsInstalled", "false", false)]
        public void ParseRegistryValueTest_InvalidValueName_Throw_InvalidRegistryValueFormatException(string registryValueString, string isRegistryValue64BitString, RegistryRootKey registryRootKey, string key, string valueName, string value, bool isRegistryValue64Bit)
        {
            var target = new RegistryValueParser();
            Assert.Throws<InvalidRegistryValueFormatException>(() =>
            {
                var actual = target.ParseRegistryValue(registryValueString, isRegistryValue64BitString);
            });
        }

        [Test()]
        [TestCase(@"[HKLM\Software\MyCompany]IsInstalled=false", "false2", RegistryRootKey.LocalMachine, @"Software\MyCompany", "IsInstalled", "false", false)]
        public void ParseRegistryValueTest_InvalidValueName_Throw_InvalidFormatedBooleanException(string registryValueString, string isRegistryValue64BitString, RegistryRootKey registryRootKey, string key, string valueName, string value, bool isRegistryValue64Bit)
        {
            var target = new RegistryValueParser();
            Assert.Throws<InvalidFormatedBooleanException>(() =>
            {
                var actual = target.ParseRegistryValue(registryValueString, isRegistryValue64BitString);
            });
        }
    }
}