using System.Collections.Generic;
using Microsoft.ConfigurationManagement.AdminConsole;
using Microsoft.ConfigurationManagement.ApplicationManagement;

namespace SccmTools.Library.Module.Services
{
    public interface ISccmApplicationProvider
    {
        IEnumerable<Application> FindApplication(string applicationName, string applicationVersion);
    }
}