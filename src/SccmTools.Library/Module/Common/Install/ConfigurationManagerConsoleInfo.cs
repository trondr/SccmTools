using System;
using System.IO;

namespace SccmTools.Library.Common.Install
{
    public class ConfigurationManagerConsoleInfo : IConfigurationManagerConsoleInfo
    {
        private string _actionExtensionsPath;

        public string ActionsExtensionsPath
        {
            get
            {
                if(string.IsNullOrWhiteSpace(_actionExtensionsPath))
                {
                    var adminUiBinFolder = Environment.GetEnvironmentVariable("SMS_ADMIN_UI_PATH");
                    if(!string.IsNullOrWhiteSpace(adminUiBinFolder) && Directory.Exists(adminUiBinFolder))
                    {
                        var adminUiBinDirectory = new DirectoryInfo(adminUiBinFolder);
                        if (adminUiBinDirectory.Parent != null)
                        {
                            if (adminUiBinDirectory.Parent.Parent != null)
                            {
                                var adminConsoleDirectory = adminUiBinDirectory.Parent.Parent;
                                var extensionPath = Path.Combine(adminConsoleDirectory.FullName, "XmlStorage", "Extensions", "Actions");
                                if (Directory.Exists(extensionPath))
                                {
                                    _actionExtensionsPath = extensionPath;
                                }
                                
                            }
                        }
                    }
                }                
                return _actionExtensionsPath;
            }
            set { _actionExtensionsPath = value; }
        }
    }
}