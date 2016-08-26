using System.Collections.Generic;
using System.Configuration.Install;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Xml.Serialization;
using Microsoft.ConfigurationManagement.AdminConsole.Schema;

namespace SccmTools.Library.Common.Install
{
    public class ConfigurationManagerContextMenuInstaller : IConfigurationManagerContextMenuInstaller
    {
        private readonly IConfigurationManagerConsoleInfo _configurationManagerConsoleInfo;

        public ConfigurationManagerContextMenuInstaller()
        {
            _configurationManagerConsoleInfo = new ConfigurationManagerConsoleInfo();
        }

        public void Install(string nodeGuid, string commandName, string command, string arguments, InstallContext installContext)
        {
            installContext.LogMessage(string.Format("Installing command '{0}' to node guid '{1}'", commandName, nodeGuid));
            if(string.IsNullOrWhiteSpace(_configurationManagerConsoleInfo.ActionsExtensionsPath) || !Directory.Exists(_configurationManagerConsoleInfo.ActionsExtensionsPath))
            {
                throw new SccmToolsException("Unable to locate Configuration Manager Console on this machine. Failed to install action command: " + commandName);
            }
            var actionDescriptionDescription = new ActionDescription
            {
                ActionClassSetting = ActionDescription.ActionClass.Executable,
                DisplayName = commandName,
                MnemonicDisplayName = commandName,
                DisplayDescription = commandName,
                ShowOn = new List<string>(2) {"ContextMenu", "DefaultContextualTab"},
                ExecutableDescription = new ExecutableDescription()
                {
                    FilePath = command,
                    Parameters = arguments
                }                
            };
            var extensionNodepath = Path.Combine(_configurationManagerConsoleInfo.ActionsExtensionsPath, nodeGuid); 
            if(!Directory.Exists(extensionNodepath))
            {
                installContext.LogMessage(string.Format("Creating action node directory '{0}'...", extensionNodepath));
                Directory.CreateDirectory(extensionNodepath);
            }
            var actionDescriptionFile = Path.Combine(extensionNodepath, commandName + ".xml");
            var serializer = new XmlSerializer(typeof(ActionDescription));
            using(var sw = new StreamWriter(actionDescriptionFile))
            {
                installContext.LogMessage(string.Format("Saving action description to file '{0}'...", actionDescriptionFile));
                serializer.Serialize(sw, actionDescriptionDescription);
            }                        
        }

        public void UnInstall(string nodeGuid, string commandName, InstallContext installContext)
        {
            installContext.LogMessage(string.Format("Uninstalling command '{0}' from node guid '{1}'", commandName, nodeGuid));
            if(!string.IsNullOrWhiteSpace(_configurationManagerConsoleInfo.ActionsExtensionsPath) && Directory.Exists(_configurationManagerConsoleInfo.ActionsExtensionsPath))
            {            
                var extensionNodepath = Path.Combine(_configurationManagerConsoleInfo.ActionsExtensionsPath, nodeGuid); 
                var actionDescriptionFile = Path.Combine(extensionNodepath, commandName + ".xml");
                if(File.Exists(actionDescriptionFile))
                {
                    installContext.LogMessage("Deleting empty action file: " + actionDescriptionFile);
                    File.Delete(actionDescriptionFile);
                }
                if((Directory.GetFiles(extensionNodepath).Length == 0) && (Directory.GetDirectories(extensionNodepath).Length == 0))
                {
                    installContext.LogMessage("Deleting empty action directory: " + extensionNodepath);
                    Directory.Delete(extensionNodepath);
                }
            }
            else
            {
                installContext.LogMessage("Nothing to uninstall. Extension directory does not exist: " + _configurationManagerConsoleInfo.ActionsExtensionsPath);
            }
        }
    }
}