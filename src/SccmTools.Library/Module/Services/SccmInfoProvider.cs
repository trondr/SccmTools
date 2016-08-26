using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using SccmTools.Library.Module.Common.Wmi;
using SmsClientLib;

namespace SccmTools.Library.Module.Services
{
    public class SccmInfoProvider : ISccmInfoProvider
    {
        private readonly IWmi _wmi;

        public SccmInfoProvider(IWmi wmi)
        {
            _wmi = wmi;
        }

        private WqlConnectionManager _sccmConnection;

        public string GetSiteId()
        {
            using(_sccmConnection = new WqlConnectionManager())
            {
                _sccmConnection.Connect(GetSiteServer());
                var queryResults = _sccmConnection.ExecuteMethod("SMS_Identification","GetSiteID",null);
                var siteId = queryResults["SiteID"].StringValue;
                return siteId;
            }            
        }

        public string GetSiteCode()
        {          
            var smsClient = new SmsClient();
            return smsClient.GetAssignedSite();            
        }

        public string GetSiteServer()
        {
            var smsClient = new SmsClient();
            return smsClient.GetCurrentManagementPoint();
        }
        
        public string GetAuthoringScopeId()
        {
            return GetScopeId();
        }

        public string GetScopeId()
        {
            var siteId = GetSiteId().Substring(1,36);
            return string.Format("ScopeId_" + siteId);
        }
    }
}