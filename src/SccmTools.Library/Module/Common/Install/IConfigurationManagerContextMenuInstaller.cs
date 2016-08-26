using System.Configuration.Install;

namespace SccmTools.Library.Module.Common.Install
{
    public interface IConfigurationManagerContextMenuInstaller
    {
        void Install(string nodeGuid, string commandName, string command, string arguments, InstallContext installContext);

        void UnInstall(string nodeGuid,string commandName, InstallContext installContext);
    }
}