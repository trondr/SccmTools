using Microsoft.Deployment.WindowsInstaller;

namespace SccmTools.Library.Common.Msi
{
    public interface IMsiHelper
    {
        string GetProperty(Database msiDatabase, string propertyName);
    }
}
