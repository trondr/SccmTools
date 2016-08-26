using System;
using Microsoft.Win32;

namespace SccmTools.Library.Module.Common.Install
{
    public class WindowsExplorerContextMenuInstaller : IWindowsExplorerContextMenuInstaller
    {
        public void Install(string commandId, string commandName, string command, string arguments)
        {
            var commandLine = string.Format("\"{0}\" {1}", command, arguments);
            var commandIdKeyPath = @"Software\Classes\Folder\shell\" + commandId;
            using (var commandIdKey = Registry.LocalMachine.CreateSubKey(commandIdKeyPath))
            {
                if (commandIdKey == null) throw new NullReferenceException("Failed to create or open registry key: " + commandIdKeyPath);
                commandIdKey.SetValue(null, commandName);
            }

            var commandKeyPath = string.Format(@"Software\Classes\Folder\shell\{0}\command", commandId);
            using (var commandKey = Registry.LocalMachine.CreateSubKey(commandKeyPath))
            {
                if (commandKey == null) throw new NullReferenceException("Failed to create or open registry key: " + commandKeyPath);                
                commandKey.SetValue(null, commandLine, RegistryValueKind.String);
            }

            var commandIdKeyPath2 = @"Software\Classes\*\shell\" + commandId;
            using (var commandIdKey2 = Registry.LocalMachine.CreateSubKey(commandIdKeyPath2))
            {
                if (commandIdKey2 == null) throw new NullReferenceException("Failed to create or open registry key: " + commandIdKeyPath2);
                commandIdKey2.SetValue(null, commandName);
            }

            var commandKeyPath2 = string.Format(@"Software\Classes\*\shell\{0}\command", commandId);
            using (var commandKey2 = Registry.LocalMachine.CreateSubKey(commandKeyPath2))
            {
                if (commandKey2 == null) throw new NullReferenceException("Failed to create or open registry key: " + commandKeyPath2);                
                commandKey2.SetValue(null, commandLine, RegistryValueKind.String);
            }
        }

        public void UnInstall(string commandId)
        {
            if (string.IsNullOrWhiteSpace(commandId)) throw new ArgumentNullException(commandId);
            var commandIdKeyPath = @"Software\Classes\Folder\shell\" + commandId;
            var commandIdKeyPath2 = @"Software\Classes\*\shell\" + commandId;
            Registry.LocalMachine.DeleteSubKeyTree(commandIdKeyPath);
            Registry.LocalMachine.DeleteSubKeyTree(commandIdKeyPath2);            
        }
    }
}