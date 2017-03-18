using Microsoft.ConfigurationManagement.ApplicationManagement;

namespace SccmTools.Library.Module.Services
{
    public interface ISccmApplication
    {
        void Save(Application application);
    }
}
