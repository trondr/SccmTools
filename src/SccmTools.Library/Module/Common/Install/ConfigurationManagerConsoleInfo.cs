using System;
using System.IO;
using Common.Logging;
using SccmTools.Library.Module.Services;

namespace SccmTools.Library.Module.Common.Install
{
    public class ConfigurationManagerConsoleInfo : IConfigurationManagerConsoleInfo
    {
        private readonly ILog _logger;

        public ConfigurationManagerConsoleInfo(ILog logger)
        {
            _logger = logger;
        }


        public string ActionsExtensionsPath
        {
            get
            {
                if(string.IsNullOrWhiteSpace(_actionExtensionsPath))
                {
                    _actionExtensionsPath = GetActionExtensionPath();
                }
                return _actionExtensionsPath;
            }
            set { _actionExtensionsPath = value; }
        }
        private string _actionExtensionsPath;


        private string GetActionExtensionPath()
        {
            var adminUiBinFolder = Environment.GetEnvironmentVariable("SMS_ADMIN_UI_PATH");
            if (string.IsNullOrWhiteSpace(adminUiBinFolder))
            {
                _logger.Warn("Environment variable SMS_ADMIN_UI_PATH is not set. Please verify that Microsoft Configuration Manager Console is installed on this machine.");
                return null;
            }
            if (!Directory.Exists(adminUiBinFolder))
            {
                _logger.WarnFormat("Admin UI bin folder '{0}' not found. Please verify that Microsoft Configuration Manager Console is installed on this machine.", adminUiBinFolder);
                return null;
            }
            var adminUiBinDirectory = new DirectoryInfo(adminUiBinFolder);
            if (adminUiBinDirectory.Parent == null)
            {
                _logger.WarnFormat("Parent of Admin UI bin folder '{0}' not found. Please verify that Microsoft Configuration Manager Console is installed on this machine.", adminUiBinDirectory.FullName);
                return null;
            }
            if (adminUiBinDirectory.Parent.Parent == null)
            {
                _logger.WarnFormat("Parent of Admin UI folder '{0}' not found. Please verify that Microsoft Configuration Manager Console is installed on this machine.", adminUiBinDirectory.Parent.FullName);
                return null;
            }
            var adminConsoleDirectory = adminUiBinDirectory.Parent.Parent;
            var actionExtensionsPath = Path.Combine(adminConsoleDirectory.FullName, "XmlStorage", "Extensions", "Actions");
            if (!Directory.Exists(actionExtensionsPath))
            {
                _logger.WarnFormat("Actions extensions folder '{0}' not found. Please verify that Microsoft Configuration Manager Console is installed on this machine.", actionExtensionsPath);
                return null;
            }
            return actionExtensionsPath;
        }
    }
}