using System;
using System.Management;

namespace SccmTools.Library.Common.Wmi
{
    public class Wmi : IWmi
    {
        public ManagementScope GetManagementScope(string machineName, string wmiNameSpace, string userName, string password)
        {
            ManagementPath path;
            if(!string.IsNullOrWhiteSpace(machineName))
            {
                path = new ManagementPath(string.Format(@"\\{0}\root\{1}", machineName, wmiNameSpace));    
            }
            else
            {
                path = new ManagementPath(string.Format(@"\\localhost\root\{0}", wmiNameSpace));
            }
            var connectionOptions = new ConnectionOptions()
            {
                Username = userName,
                Password = password,
                Authentication = AuthenticationLevel.PacketPrivacy,
                Impersonation = ImpersonationLevel.Impersonate,
            };
            var managementScope = new ManagementScope(path, connectionOptions);
            try
            {
                managementScope.Connect();
            }
            catch (Exception ex)
            {
                throw new SccmToolsException("Failed to connect to management scope: " + path.Path, ex);
            }
            return managementScope;
        }
    }
}