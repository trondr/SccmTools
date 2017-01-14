using System.CodeDom;
using Microsoft.ConfigurationManagement.DesiredConfiguration;
using Microsoft.ConfigurationManagement.DesiredConfigurationManagement;

namespace SccmTools.Library.Module.Commands.CreateApplication2
{
    public interface IRegistryValueParser
    {
        RegistryValue ParseRegistryValue(string registryValueString, string isRegistryValue64BitString);
        string GetRootKeyString(RegistryRootKey registryValueRootKey);
    }
}