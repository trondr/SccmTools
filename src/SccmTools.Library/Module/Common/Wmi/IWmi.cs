using System.Management;

namespace SccmTools.Library.Common.Wmi
{
    public interface IWmi
    {        
        ManagementScope GetManagementScope(string machineName, string wmiNameSpace, string userName, string password);
    }
}
