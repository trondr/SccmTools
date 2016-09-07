using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using SccmTools.Library.Module.Common.IO;
using SccmTools.Library.Module.Services;
using Icon = Microsoft.ConfigurationManagement.ApplicationManagement.Icon;

namespace SccmTools.Library.Module.Commands.CreateApplication2
{
    public class PackageDefinitionProvider : IPackageDefinitionProvider
    {
        private readonly IIniFileOperation _iniFileOperation;
        private readonly IProductCodeProvider2 _productCodeProvider;        

        public PackageDefinitionProvider(IIniFileOperation iniFileOperation, IProductCodeProvider2 productCodeProvider)
        {
            _iniFileOperation = iniFileOperation;
            _productCodeProvider = productCodeProvider;            
        }

        public PackageDefinition2 ReadPackageDefinition(string packageDefinitionFileName)
        {
            string name = GetName(packageDefinitionFileName);
            string version = GetVersion(packageDefinitionFileName);
            string publisher = GetPublisher(packageDefinitionFileName);
            string comment = GetComment(packageDefinitionFileName);
            string language = GetLanguage(packageDefinitionFileName);
            string installCommandLine = GetInstallCommandLine(packageDefinitionFileName);
            string unInstallCommandLine = GetUnInstallCommandLine(packageDefinitionFileName);
            Icon icon = GetIcon(packageDefinitionFileName);
            string[] msiProductCodes = GetMsiProductCodes(packageDefinitionFileName).ToArray();
            DetectionMethodFile[] files = GetFiles(packageDefinitionFileName).ToArray();
            string contentDirectory = GetContentDirectory(packageDefinitionFileName);
            var packageDefinition = new PackageDefinition2(name, version, publisher, comment, language, installCommandLine, unInstallCommandLine, icon, msiProductCodes, files, contentDirectory);
            return packageDefinition;            
        }

        public void WritePackageDefinition(string packageDefinitionFileName, PackageDefinition2 packageDefinition)
        {
            SetValue(packageDefinitionFileName,"PDF","Version","2.0");
            SetName(packageDefinitionFileName, packageDefinition.Name);
            SetVersion(packageDefinitionFileName, packageDefinition.Version);
            SetPublisher(packageDefinitionFileName, packageDefinition.Publisher);
            SetComment(packageDefinitionFileName, packageDefinition.Comment);
            SetLanguage(packageDefinitionFileName, packageDefinition.Language);
            SetValue(packageDefinitionFileName,"Package Definition","Programs","INSTALL,UNINSTALL");
            SetInstallCommandLine(packageDefinitionFileName, packageDefinition.InstallCommandLine);
            SetUnInstallCommandLine(packageDefinitionFileName, packageDefinition.UnInstallCommandLine);
            //SetIcon(packageDefinitionFileName, packageDefinition.Icon);
            SetMsiProductCodes(packageDefinitionFileName, packageDefinition.MsiProductCodes);
            SetFiles(packageDefinitionFileName, packageDefinition.Files);
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

        private void SetUnInstallCommandLine(string packageDefinitionFileName, string packageDefinitionUnInstallCommandLine)
        {
            SetValue(packageDefinitionFileName, "UNINSTALL", "CommandLine", packageDefinitionUnInstallCommandLine);
        }

        private Icon GetIcon(string packageDefinitionFileName)
        {
            var iconPath = GetValue(packageDefinitionFileName, "INSTALL", "Icon", true);
            Icon icon = null;
            var directoryInfo = new FileInfo(packageDefinitionFileName).Directory;
            var packageDirectory = string.Empty;
            if (directoryInfo != null)
            {
                packageDirectory = directoryInfo.FullName;
            }
            iconPath = Path.Combine(packageDirectory, iconPath);
            if (File.Exists(iconPath))
            {
                icon = new Icon(Image.FromFile(iconPath));
            }
            return icon;
        }

        private void SetIcon(string packageDefinitionFileName, Icon packageDefinitionIcon)
        {
            // Do nothing.
        }
        
        private IEnumerable<string> GetMsiProductCodes(string packageDefinitionFileName)
        {
            var contentDirectory = GetContentDirectory(packageDefinitionFileName);
            var productCodes = GetValues(packageDefinitionFileName, "DetectionMethod", @"MsiProductCode\d*", true);
            var msiProductCodeCount = 0;
            if (productCodes.Length > 0)
            {
                foreach (var productCode in productCodes)
                {
                    var msiProductCode = _productCodeProvider.GetProductCodeFromText(productCode);
                    msiProductCodeCount++;
                    yield return msiProductCode;
                }
            }
            else
            {                
                var msiProductCodes = _productCodeProvider.GetProductCodesFromMsiFileSearch(contentDirectory);
                foreach (var msiProductCode in msiProductCodes)
                {
                    msiProductCodeCount++;
                    yield return msiProductCode;
                }
            }
            if (msiProductCodeCount == 0)
            {
                throw new SccmToolsException(string.Format(@"ProductCode(s) was not found in the section value [DetectionMethod]MsiProductCode\d* in package definition file '{0}' or from automatically searching for a msi file and its product code in content directory '{1}'.", packageDefinitionFileName, contentDirectory));
            }
        }

        private void SetMsiProductCodes(string packageDefinitionFileName, string[] packageDefinitionMsiProductCodes)
        {
            var index = 0;
            foreach (var packageDefinitionMsiProductCode in packageDefinitionMsiProductCodes)
            {
                index++;
                SetValue(packageDefinitionFileName, "DetectionMethod", "MsiProductCode" + index, packageDefinitionMsiProductCode);
            }
        }


        private IEnumerable<DetectionMethodFile> GetFiles(string packageDefinitionFileName)
        {
            var files = GetValues(packageDefinitionFileName, "DetectionMethod", @"File\d*", true);
            foreach (var file in files)
            {
                var detectionMethodFile = JsonConvert.DeserializeObject<DetectionMethodFile>(file);
                yield return detectionMethodFile;
            }            
        }

        private void SetFiles(string packageDefinitionFileName, DetectionMethodFile[] detectionMethodFiles)
        {
            var index = 0;
            foreach (var detectionMethodFile in detectionMethodFiles)
            {
                index++;
                var detectionMethodFileString = JsonConvert.SerializeObject(detectionMethodFile);
                SetValue(packageDefinitionFileName, "DetectionMethod", "File" + index, detectionMethodFileString);
            }
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

        private string[] GetValues(string fileName, string section, string keyPattern, bool allowNullOrEmpty)
        {
            var values = _iniFileOperation.ReadKeys(fileName,section,keyPattern);
            if (!allowNullOrEmpty && values.Length == 0)
            { 
                throw new SccmToolsException(string.Format("Key with pattern'[{0}]{1}' has not bee specified in package definition file '{2}'.", section, keyPattern, fileName));
            }
            return values;
        }
    }
}