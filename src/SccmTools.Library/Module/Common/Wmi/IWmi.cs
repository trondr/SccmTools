using System.Management;

namespace SccmTools.Library.Module.Common.Wmi
{
    public interface IWmi
    {        
        ManagementScope GetManagementScope(string machineName, string wmiNameSpace, string userName, string password);
    }
}
