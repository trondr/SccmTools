namespace SccmTools.Library.Module.Common.Install
{
    public interface IWindowsExplorerContextMenuInstaller
    {
        void Install(string commandId, string commandName, string command, string arguments);

        void UnInstall(string commandId);
    }
}
