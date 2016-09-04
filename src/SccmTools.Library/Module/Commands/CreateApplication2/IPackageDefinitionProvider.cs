using System.Drawing;
using System.IO;
using SccmTools.Library.Module.Common.IO;
using SccmTools.Library.Module.Services;
using Icon = Microsoft.ConfigurationManagement.ApplicationManagement.Icon;

namespace SccmTools.Library.Module.Commands.CreateApplication2
{
    public interface IPackageDefinitionProvider
    {
        PackageDefinition2 GetPackageDefinition(string packageDefinitionFileName);
    }

    public class PackageDefinitionProvider : IPackageDefinitionProvider
    {
        private readonly IIniFileOperation _iniFileOperation;

        public PackageDefinitionProvider(IIniFileOperation iniFileOperation)
        {
            _iniFileOperation = iniFileOperation;
        }

        public PackageDefinition2 GetPackageDefinition(string packageDefinitionFileName)
        {
            string name = GetName(packageDefinitionFileName);
            string version = GetVersion(packageDefinitionFileName);
            string publisher = GetPublisher(packageDefinitionFileName);
            string comment = GetComment(packageDefinitionFileName);
            string language = GetLanguage(packageDefinitionFileName);
            string installCommandLine = GetInstallCommandLine(packageDefinitionFileName);
            string unInstallCommandLine = GetUnInstallCommandLine(packageDefinitionFileName);
            Icon icon = GetIcon(packageDefinitionFileName);
            string[] msiProductCodes = GetMsiProductCodes(packageDefinitionFileName);
            string[] files = GetFiles(packageDefinitionFileName);
            string contentDirectory = GetContentDirectory(packageDefinitionFileName);
            var packageDefinition = new PackageDefinition2(name,version,publisher,comment,language,installCommandLine,unInstallCommandLine,icon,msiProductCodes,files,contentDirectory);
            return packageDefinition;            
        }

        private string GetName(string packageDefinitionFileName)
        {
            var name = GetValue(packageDefinitionFileName, "Package Definition", "Name", false);
            return name;
        }

        private string GetVersion(string packageDefinitionFileName)
        {
            var version = GetValue(packageDefinitionFileName, "Package Definition", "Version", false);
            return version;
        }

        private string GetPublisher(string packageDefinitionFileName)
        {
            var publisher = GetValue(packageDefinitionFileName, "Package Definition", "Publisher", false);
            return publisher;
        }

        private string GetComment(string packageDefinitionFileName)
        {
            var comment = GetValue(packageDefinitionFileName, "Package Definition", "Comment", false);
            return comment;
        }

        private string GetLanguage(string packageDefinitionFileName)
        {
            var language = GetValue(packageDefinitionFileName, "Package Definition", "Language", false);
            return language;
        }

        private string GetInstallCommandLine(string packageDefinitionFileName)
        {
            var installCommandLine = GetValue(packageDefinitionFileName, "INSTALL", "CommandLine", false);
            return installCommandLine;
        }

        private string GetUnInstallCommandLine(string packageDefinitionFileName)
        {
            var installCommandLine = GetValue(packageDefinitionFileName, "UNINSTALL", "CommandLine", false);
            return installCommandLine;
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

        private string[] GetMsiProductCodes(string packageDefinitionFileName)
        {
            throw new System.NotImplementedException();
        }

        private string[] GetFiles(string packageDefinitionFileName)
        {
            throw new System.NotImplementedException();
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
    }
}