using Microsoft.ConfigurationManagement.DesiredConfigurationManagement;

namespace SccmTools.Library.Module.Commands.CreateApplication
{
    public interface IRegistryValueParser
    {
        RegistryValue ParseRegistryValue(string registryValueString, string isRegistryValue64BitString);
        string GetRootKeyString(RegistryRootKey registryValueRootKey);
    }
}