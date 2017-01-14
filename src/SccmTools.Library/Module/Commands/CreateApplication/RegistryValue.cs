using Microsoft.ConfigurationManagement.DesiredConfigurationManagement;

namespace SccmTools.Library.Module.Commands.CreateApplication
{
    public class RegistryValue
    {
        public RegistryValue(RegistryRootKey rootKey, string key, string valueName, string value, bool is64Bit)
        {
            RootKey = rootKey;
            Key = key;
            ValueName = valueName;
            Value = value;
            Is64Bit = is64Bit;
        }

        public RegistryRootKey RootKey { get; }
        public string Key { get; }
        public string ValueName { get; }
        public bool Is64Bit { get; }
        public string Value { get; }
    }
}