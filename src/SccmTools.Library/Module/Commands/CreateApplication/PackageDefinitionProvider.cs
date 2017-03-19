using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using SccmTools.Library.Module.Common.IO;
using SccmTools.Library.Module.Services;
using Icon = Microsoft.ConfigurationManagement.ApplicationManagement.Icon;

namespace SccmTools.Library.Module.Commands.CreateApplication
{
    public class PackageDefinitionProvider : IPackageDefinitionProvider
    {
        private readonly IIniFileOperation _iniFileOperation;
        private readonly IProductCodeProvider _productCodeProvider;
        private readonly IRegistryValueParser _regfRegistryValueParser;

        public PackageDefinitionProvider(IIniFileOperation iniFileOperation, 
            IProductCodeProvider productCodeProvider, IRegistryValueParser regfRegistryValueParser)
        {
            _iniFileOperation = iniFileOperation;
            _productCodeProvider = productCodeProvider;
            _regfRegistryValueParser = regfRegistryValueParser;
        }

        public PackageDefinition ReadPackageDefinition(string packageDefinitionFileName)
        {
            var name = GetName(packageDefinitionFileName);
            var version = GetVersion(packageDefinitionFileName);
            var publisher = GetPublisher(packageDefinitionFileName);
            var comment = GetComment(packageDefinitionFileName);
            var language = GetLanguage(packageDefinitionFileName);
            var installCommandLine = GetInstallCommandLine(packageDefinitionFileName);
            var unInstallCommandLine = GetUnInstallCommandLine(packageDefinitionFileName);
            var icon = GetIcon(packageDefinitionFileName);
            var msiProductCode = GetMsiProductCode(packageDefinitionFileName);
            var registryValue = GetRegistryValue(packageDefinitionFileName);
            var registryValueIs64Bit = GetRegistryValueIs64Bit(packageDefinitionFileName);
            var contentDirectory = GetContentDirectory(packageDefinitionFileName);
            var dependencies = GetDependencies(packageDefinitionFileName);
            var packageDefinition = new PackageDefinition(
                                                    name, 
                                                    version, 
                                                    publisher, 
                                                    comment, 
                                                    language,
                                                    installCommandLine, 
                                                    unInstallCommandLine, 
                                                    icon, msiProductCode, 
                                                    registryValue,
                                                    registryValueIs64Bit,
                                                    contentDirectory, 
                                                    dependencies);
            return packageDefinition;
        }

        private IEnumerable<Dependency> GetDependencies(string packageDefinitionFileName)
        {
            const string dependenciesSectionName = "Dependencies";

            var dependenciesExist = _iniFileOperation.SectionExists(packageDefinitionFileName, dependenciesSectionName);
            if(!dependenciesExist)
                yield break;

            var dependencyKeys = _iniFileOperation.ReadKeys(packageDefinitionFileName, dependenciesSectionName, @"Dependency\d+");
            foreach (var dependencyKeyValuePair in dependencyKeys)
            {
                var dependencyValue = dependencyKeyValuePair.Value;
                var dependency = GetDependency(dependencyValue);
                if (dependency != null)
                {
                    yield return dependency;
                }
            }
        }

        private Dependency GetDependency(string dependencyValue)
        {
            var regEx = new Regex(@"""(.+)"";""(.+)""");
            var match = regEx.Match(dependencyValue);
            if (match.Success)
            {
                var applicationName = match.Groups[1].Value;
                var applicationVersion = match.Groups[2].Value;
                var dependency = new Dependency()
                {
                    ApplicationName = applicationName,
                    ApplicationVersion = applicationVersion
                };
                return dependency;
            }
            return null;
        }

        private bool GetRegistryValueIs64Bit(string packageDefinitionFileName)
        {
            var isRegistryValue64BitString = GetValue(packageDefinitionFileName, "DetectionMethod","RegistryValueIs64Bit", true);
            if (string.IsNullOrWhiteSpace(isRegistryValue64BitString))
                return true;
            switch (isRegistryValue64BitString)
            {
                case "true":
                case "1":
                    return true;
                case "false":
                case "0":
                    return false;
                default:
                    return true;
            }
        }

        private RegistryValue GetRegistryValue(string packageDefinitionFileName)
        {
            var registryValueString = GetValue(packageDefinitionFileName, "DetectionMethod", "RegistryValue", true);
            var isRegistryValue64BitString = GetValue(packageDefinitionFileName, "DetectionMethod", "RegistryValueIs64Bit", true);
            if (string.IsNullOrWhiteSpace(isRegistryValue64BitString)) isRegistryValue64BitString = "true";
            if (!string.IsNullOrWhiteSpace(registryValueString))
            {
                var registryValue =  _regfRegistryValueParser.ParseRegistryValue(registryValueString, isRegistryValue64BitString);
                return registryValue;    
            }
            return null;
        }

        public void WritePackageDefinition(string packageDefinitionFileName, PackageDefinition packageDefinition)
        {
            SetValue(packageDefinitionFileName, "PDF", "Version", "2.0");
            SetName(packageDefinitionFileName, packageDefinition.Name);
            SetVersion(packageDefinitionFileName, packageDefinition.Version);
            SetPublisher(packageDefinitionFileName, packageDefinition.Publisher);
            SetComment(packageDefinitionFileName, packageDefinition.Comment);
            SetLanguage(packageDefinitionFileName, packageDefinition.Language);
            SetValue(packageDefinitionFileName, "Package Definition", "Programs", "INSTALL,UNINSTALL");
            SetInstallCommandLine(packageDefinitionFileName, packageDefinition.InstallCommandLine);
            SetUnInstallCommandLine(packageDefinitionFileName, packageDefinition.UnInstallCommandLine);
            //SetIcon(packageDefinitionFileName, packageDefinition.Icon);
            SetMsiProductCode(packageDefinitionFileName, packageDefinition.MsiProductCode);
            SetRegistryValue(packageDefinitionFileName, packageDefinition.RegistryValue);
            //SetContentDirectory(packageDefinitionFileName,packageDefinition.ContentDirectory);
        }

        private string GetName(string packageDefinitionFileName)
        {
            var name = GetValue(packageDefinitionFileName, "Package Definition", "Name", false);
            return name;
        }

        private void SetName(string packageDefinitionFileName, string packageDefinitionName)
        {
            SetValue(packageDefinitionFileName, "Package Definition", "Name", packageDefinitionName);
        }

        private string GetVersion(string packageDefinitionFileName)
        {
            var version = GetValue(packageDefinitionFileName, "Package Definition", "Version", false);
            return version;
        }

        private void SetVersion(string packageDefinitionFileName, string packageDefinitionVersion)
        {
            SetValue(packageDefinitionFileName, "Package Definition", "Version", packageDefinitionVersion);
        }

        private string GetPublisher(string packageDefinitionFileName)
        {
            var publisher = GetValue(packageDefinitionFileName, "Package Definition", "Publisher", false);
            return publisher;
        }

        private void SetPublisher(string packageDefinitionFileName, string packageDefinitionPublisher)
        {
            SetValue(packageDefinitionFileName, "Package Definition", "Publisher", packageDefinitionPublisher);
        }

        private string GetComment(string packageDefinitionFileName)
        {
            var comment = GetValue(packageDefinitionFileName, "Package Definition", "Comment", false);
            return comment;
        }

        private void SetComment(string packageDefinitionFileName, string packageDefinitionComment)
        {
            SetValue(packageDefinitionFileName, "Package Definition", "Comment", packageDefinitionComment);
        }

        private string GetLanguage(string packageDefinitionFileName)
        {
            var language = GetValue(packageDefinitionFileName, "Package Definition", "Language", false);
            return language;
        }

        private void SetLanguage(string packageDefinitionFileName, string packageDefinitionLanguage)
        {
            SetValue(packageDefinitionFileName, "Package Definition", "Language", packageDefinitionLanguage);
        }

        private string GetInstallCommandLine(string packageDefinitionFileName)
        {
            var installCommandLine = GetValue(packageDefinitionFileName, "INSTALL", "CommandLine", false);
            return installCommandLine;
        }

        private void SetInstallCommandLine(string packageDefinitionFileName, string packageDefinitionInstallCommandLine)
        {
            SetValue(packageDefinitionFileName, "INSTALL", "CommandLine", packageDefinitionInstallCommandLine);
        }

        private string GetUnInstallCommandLine(string packageDefinitionFileName)
        {
            var installCommandLine = GetValue(packageDefinitionFileName, "UNINSTALL", "CommandLine", false);
            return installCommandLine;
        }

        private void SetUnInstallCommandLine(string packageDefinitionFileName,
            string packageDefinitionUnInstallCommandLine)
        {
            SetValue(packageDefinitionFileName, "UNINSTALL", "CommandLine", packageDefinitionUnInstallCommandLine);
        }

        private Icon GetIcon(string packageDefinitionFileName)
        {
            var iconPath = GetValue(packageDefinitionFileName, "INSTALL", "Icon", true);
            if (string.IsNullOrEmpty(iconPath))
                return null;
            Icon icon = null;
            var directoryInfo = new FileInfo(packageDefinitionFileName).Directory;
            var packageDirectory = string.Empty;
            if (directoryInfo != null)
            {
                packageDirectory = directoryInfo.FullName;
            }
            iconPath = Path.Combine(packageDirectory, iconPath);
            if (System.IO.File.Exists(iconPath))
            {
                icon = new Icon(Image.FromFile(iconPath));
            }
            return icon;
        }

        private void SetIcon(string packageDefinitionFileName, Icon packageDefinitionIcon)
        {
            // Do nothing.
        }

        private string GetMsiProductCode(string packageDefinitionFileName)
        {
            var contentDirectory = GetContentDirectory(packageDefinitionFileName);
            var productCode = GetValue(packageDefinitionFileName, "DetectionMethod", @"MsiProductCode", true);
            var msiProductCode = productCode;
            if (!string.IsNullOrWhiteSpace(msiProductCode))
            {
                msiProductCode = _productCodeProvider.GetProductCodeFromText(productCode);                
            }
            if (string.IsNullOrWhiteSpace(msiProductCode))
            {
                msiProductCode = _productCodeProvider.GetProductCodeFromMsiFileSearch(contentDirectory);                
            }
            return msiProductCode;
        }

        private void SetMsiProductCode(string packageDefinitionFileName, string packageDefinitionMsiProductCode)
        {
            SetValue(packageDefinitionFileName, "DetectionMethod", "MsiProductCode", packageDefinitionMsiProductCode);
        }


        private void SetRegistryValue(string packageDefinitionFileName, RegistryValue registryValue)
        {
            var rootkeyString = _regfRegistryValueParser.GetRootKeyString(registryValue.RootKey);

            var registryValueString = string.Format(@"[{0}\{1}]{2}={3}", rootkeyString, registryValue.Key,
                registryValue.ValueName, registryValue.Value);

            SetValue(packageDefinitionFileName, "DetectionMethod", "RegistryValue", registryValueString);            
        }

        public string GetContentDirectory(string packageDefinitionFileName)
        {
            var packageDefinitionFile = new FileInfo(packageDefinitionFileName);
            var contentDirectory = packageDefinitionFile.Directory;
            if (contentDirectory == null) { throw new SccmToolsException("Failed to derive parent directory for package definition file: " + packageDefinitionFileName); }
            return contentDirectory.FullName;
        }

        private string GetValue(string fileName, string section, string key, bool allowNullOrEmpty)
        {
            var value = _iniFileOperation.Read(fileName, section, key);
            if (!allowNullOrEmpty && string.IsNullOrWhiteSpace(value))
            {
                throw new SccmToolsException(string.Format("'[{0}]{1}' has not bee specified in package definition file '{2}'.", section, key, fileName));
            }
            return value;
        }

        private void SetValue(string fileName, string section, string key, string value)
        {
            _iniFileOperation.Write(fileName, section, key, value);
        }

        private KeyValuePair<string,string>[] GetKeyValues(string fileName, string section, string keyPattern, bool allowNullOrEmpty)
        {
            var keyValues = _iniFileOperation.ReadKeys(fileName, section, keyPattern);
            if (!allowNullOrEmpty && keyValues.Length == 0)
            {
                throw new SccmToolsException(string.Format("Key with pattern'[{0}]{1}' has not bee specified in package definition file '{2}'.", section, keyPattern, fileName));
            }
            return keyValues;
        }
    }
}