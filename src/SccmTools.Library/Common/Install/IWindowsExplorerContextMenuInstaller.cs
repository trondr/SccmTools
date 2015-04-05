using System.Reflection;

namespace SccmTools.Library.Common.Install
{
    public interface IWindowsExplorerContextMenuInstaller
    {
        void Install(string commandId, string commandName, string command, string arguments);

        void UnInstall(string commandId);
    }
}
