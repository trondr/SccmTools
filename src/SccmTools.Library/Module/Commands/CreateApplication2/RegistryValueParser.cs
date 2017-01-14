using System;
using System.Text.RegularExpressions;
using Microsoft.ConfigurationManagement.DesiredConfigurationManagement;

namespace SccmTools.Library.Module.Commands.CreateApplication2
{
    public class RegistryValueParser : IRegistryValueParser
    {
        public RegistryValue ParseRegistryValue(string registryValueString, string isRegistryValue64BitString)
        {
            var registryValueRegexPattern = @"\[(.+?)\](.+?)=(.+)";
            var registryValueRegex = new Regex(registryValueRegexPattern);
            var match = registryValueRegex.Match(registryValueString);
            if (match.Success)
            {
                var keyPart = match.Groups[1].Value;
                var valueName = match.Groups[2].Value;
                var value = match.Groups[3].Value;
                var keyTupple = GetKey(keyPart);
                var is64Bit = GetIs64Bit(isRegistryValue64BitString);
                var registryValue = new RegistryValue(keyTupple.Item1, keyTupple.Item2, valueName, value, is64Bit);
                return registryValue;
            }
            throw new InvalidRegistryValueFormatException(string.Format("Invalid formated registry value string. Registry value string '{0}' does not match the regular expression: {1}", registryValueString, registryValueRegexPattern));
        }

        public string GetRootKeyString(RegistryRootKey registryValueRootKey)
        {
            switch (registryValueRootKey)
            {
                case RegistryRootKey.ClassesRoot:
                    return "HKCR";                    
                case RegistryRootKey.CurrentConfig:
                    return "HKCC";
                case RegistryRootKey.CurrentUser:
                    return "HKCU";
                case RegistryRootKey.LocalMachine:
                    return "HKLM";
                case RegistryRootKey.Users:
                    return "HKU";
                default:
                    throw new ArgumentOutOfRangeException(nameof(registryValueRootKey), registryValueRootKey, null);
            }
        }

        private bool GetIs64Bit(string isRegistryValue64BitString)
        {
            switch (isRegistryValue64BitString)
            {
                case "true":
                case "1":
                    return true;
                case "false":
                case "0":
                    return false;
                default:
                    throw new InvalidFormatedBooleanException(string.Format("Invalid formated boolean string '{0}', valid values: true,false,1,0", isRegistryValue64BitString));
            }
        }

        private Tuple<RegistryRootKey, string> GetKey(string keyPart)
        {
            var keyPattern = @"(HKLM|HKEY_LOCAL_MACHINE|HKCU|HKEY_CURRENT_USER|HKCR|HKEY_CLASSES_ROOT|HKU|HKEY_USERS|HKCC|HKEY_CURRENT_CONFIG)\\(.+)";
            var keyRegEx = new Regex(keyPattern);
            var match = keyRegEx.Match(keyPart);
            if (match.Success)
            {
                var rootKeyString = match.Groups[1].Value;
                var key = match.Groups[2].Value;
                var rootKey = GetRootKey(rootKeyString);
                return new Tuple<RegistryRootKey, string>(rootKey, key);
            }
            throw new InvalidRegistryKeyFormatException(string.Format("Invalid formated registry key path '{0}'. Registry key string does not match regular expression:  {1}", keyPart, keyPattern));
        }

        private RegistryRootKey GetRootKey(string rootKeyString)
        {
            switch (rootKeyString)
            {
                case "HKLM":
                case "HKEY_LOCAL_MACHINE":
                    return RegistryRootKey.LocalMachine;
                case "HKCU":
                case "HKEY_CURRENT_USER":
                    return RegistryRootKey.CurrentUser;
                case "HKCR":
                case "HKEY_CLASSES_ROOT":
                    return RegistryRootKey.ClassesRoot;
                case "HKU":
                case "HKEY_USERS":
                    return RegistryRootKey.Users;
                case "HKCC":
                case "HKEY_CURRENT_CONFIG":
                    return RegistryRootKey.CurrentConfig;
                default:
                    throw new InvalidRegistryRootKeyException(rootKeyString);
            }
        }
    }
}