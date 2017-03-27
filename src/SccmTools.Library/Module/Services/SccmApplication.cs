using System;
using System.Management;
using Common.Logging;
using Microsoft.ConfigurationManagement.ApplicationManagement;
using Microsoft.ConfigurationManagement.ApplicationManagement.Serialization;
using SccmTools.Library.Module.Common.Wmi;

namespace SccmTools.Library.Module.Services
{
    public class SccmApplication : ISccmApplication
    {
        private readonly ISccmInfoProvider _sccmInfoProvider;
        private readonly IWmi _wmi;
        private readonly ILog _logger;

        public SccmApplication(ISccmInfoProvider sccmInfoProvider, IWmi wmi, ILog logger)
        {
            _sccmInfoProvider = sccmInfoProvider;
            _wmi = wmi;
            _logger = logger;
        }
        
        public void Save(Application application)
        {
            if (application == null) throw new ArgumentNullException(nameof(application));

            var appDefintionXml = SccmSerializer.SerializeToString(application);
            if (_logger.IsDebugEnabled) _logger.DebugFormat("Application definition XML:{0}{1}", Environment.NewLine, appDefintionXml);

            var siteServer = _sccmInfoProvider.GetSiteServer();
            var siteCode = _sccmInfoProvider.GetSiteCode();
            var scope = _wmi.GetManagementScope(siteServer, @"sms\site_" + siteCode, null, null);
            var path = new ManagementPath("SMS_Application");
            var options = new ObjectGetOptions();
            using (var applicationClass = new ManagementClass(scope, path, options))
            {
                using (var applicationInstance = applicationClass.CreateInstance())
                {
                    if (applicationInstance == null) throw new SccmToolsException("Application instance is null.");
                    applicationInstance.Properties["SDMPackageXML"].Value = appDefintionXml;
                    applicationInstance.Put();
                    applicationInstance.Get();
                }
            }
        }
    }
}