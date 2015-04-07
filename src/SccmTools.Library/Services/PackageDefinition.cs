using System.Drawing;
using System.IO;
using SccmTools.Library.Common.IO;
using Icon = Microsoft.ConfigurationManagement.ApplicationManagement.Icon;

namespace SccmTools.Library.Services
{
    public class PackageDefinition : IPackageDefinition
    {
        private readonly string _packageDefinitionFile;
        private readonly IIniFileOperation _iniFileOperation;
        private string _name;
        private string _version;
        private string _publisher;
        private string _comment;
        private string _language;
        private string _installCommandLine;
        private string _unInstallCommandLine;
        private Icon _icon;

        public PackageDefinition(string packageDefinitionFile, IIniFileOperation iniFileOperation)
        {
            _packageDefinitionFile = packageDefinitionFile;
            _iniFileOperation = iniFileOperation;
        }

        public string Name
        {
            get
            {
                if(string.IsNullOrWhiteSpace(_name))
                {
                    _name = GetValue("Package Definition", "Name", false);
                }
                return _name;
            }
            set { _name = value; }
        }

        public string Version
        {
            get
            {
                if(string.IsNullOrWhiteSpace(_version))
                {
                    _version = GetValue("Package Definition", "Version", false);
                    
                }
                return _version;
            }
            set { _version = value; }
        }

        public string Publisher
        {
            get
            {
                if(string.IsNullOrWhiteSpace(_publisher))
                {
                    _publisher = GetValue("Package Definition", "Publisher", false);
                    
                }
                return _publisher;
            }
            set { _publisher = value; }
        }

        public string Comment
        {
            get
            {
                if(string.IsNullOrWhiteSpace(_comment))
                {
                    _comment = GetValue("Package Definition", "Comment", true);
                    
                }
                return _comment;
            }
            set { _comment = value; }
        }

        public string Language
        {
            get
            {
                if(string.IsNullOrWhiteSpace(_language))
                {
                    _language = GetValue("Package Definition", "Language", false);                    
                }
                return _language;
            }
            set { _language = value; }
        }

        public string InstallCommandLine
        {
            get
            {
                if(string.IsNullOrWhiteSpace(_installCommandLine))
                {
                    _installCommandLine = GetValue("INSTALL", "CommandLine", false);                    
                }
                return _installCommandLine;
            }
            set { _installCommandLine = value; }
        }

        public string UnInstallCommandLine
        {
            get
            {
                if(string.IsNullOrWhiteSpace(_unInstallCommandLine))
                {
                    _unInstallCommandLine = GetValue("UNINSTALL", "CommandLine", false);                    
                }
                return _unInstallCommandLine;
            }
            set { _unInstallCommandLine = value; }
        }

        public Icon Icon
        {
            get
            {
                if(_icon == null)
                {
                    var iconPath = GetValue("INSTALL", "Icon", true);
                    if(!string.IsNullOrWhiteSpace(iconPath))
                    {
                        var directoryInfo = new FileInfo(_packageDefinitionFile).Directory;
                        var packageDirectory = string.Empty;
                        if (directoryInfo != null)
                        {
                            packageDirectory = directoryInfo.FullName;
                        }
                        iconPath = Path.Combine(packageDirectory, iconPath);
                        if(File.Exists(iconPath))
                        {
                            _icon = new Icon(Image.FromFile(iconPath));
                        }
                    }
                }
                return _icon;
            }
            set { _icon = value; }
        }

        string GetValue(string section, string key, bool allowNullOrEmpty)
        {
            var value = _iniFileOperation.Read(_packageDefinitionFile, section, key);
            if(!allowNullOrEmpty && string.IsNullOrWhiteSpace(value))
            { 
                throw new SccmToolsException(string.Format("'[{0}]{1}' has not bee specified in package definition file '{2}'.", section, key, _packageDefinitionFile));
            }
            return value;
        }
    }
}