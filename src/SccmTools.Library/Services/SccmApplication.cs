using System;
using System.Management;
using SccmTools.Library.Common.Wmi;

namespace SccmTools.Library.Services
{
    public class SccmApplication : ISccmApplication
    {
        private readonly ISccmInfoProvider _sccmInfoProvider;
        private readonly IWmi _wmi;

        public SccmApplication(ISccmInfoProvider sccmInfoProvider, IWmi wmi)
        {
            _sccmInfoProvider = sccmInfoProvider;
            _wmi = wmi;
        }

        public void Save(string pakageDefinitionXml)
        {
            if (string.IsNullOrWhiteSpace(pakageDefinitionXml)) throw new ArgumentNullException("pakageDefinitionXml");
            var siteServer = _sccmInfoProvider.GetSiteServer();
            var siteCode = _sccmInfoProvider.GetSiteCode();
            var scope = _wmi.GetManagementScope(siteServer,@"sms\site_" + siteCode,null,null);
            var path = new ManagementPath("SMS_Application");
            var options = new ObjectGetOptions();
            using (var applicationClass = new ManagementClass(scope, path, options))
            {
                using(var applicationInstance = applicationClass.CreateInstance())
                {
                    if (applicationInstance == null) throw new SccmToolsException("Application instance is null.");
                    applicationInstance.Properties["SDMPackageXML"].Value = pakageDefinitionXml;
                    applicationInstance.Put();
                    applicationInstance.Get();
                }
            }
        }
    }
}