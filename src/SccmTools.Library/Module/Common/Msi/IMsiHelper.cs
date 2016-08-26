using Microsoft.Deployment.WindowsInstaller;

namespace SccmTools.Library.Module.Common.Msi
{
    public interface IMsiHelper
    {
        string GetProperty(Database msiDatabase, string propertyName);
    }
}
