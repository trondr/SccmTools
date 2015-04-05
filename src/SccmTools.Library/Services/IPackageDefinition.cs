using Microsoft.ConfigurationManagement.ApplicationManagement;

namespace SccmTools.Library.Services
{
    public interface IPackageDefinition
    {
        string Name { get; set; }

        string Version { get; set; }

        string Publisher { get; set; }

        string Comment { get; set; }

        string Language { get; set; }

        string InstallCommandLine { get; set; }

        string UnInstallCommandLine { get; set; }

        Icon Icon { get; set; }
    }
}