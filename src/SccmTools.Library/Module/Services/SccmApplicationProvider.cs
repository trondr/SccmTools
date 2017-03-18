using System;
using System.Collections.Generic;
using Common.Logging;
using Microsoft.ConfigurationManagement.AdminConsole.AppManFoundation;
using Microsoft.ConfigurationManagement.ApplicationManagement;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using SccmTools.Library.Infrastructure.LifeStyles;

namespace SccmTools.Library.Module.Services
{
    [Singleton]
    public class SccmApplicationProvider : ISccmApplicationProvider, IDisposable
    {
        private readonly ISccmInfoProvider _sccmInfoProvider;
        private readonly ILog _logger;

        public SccmApplicationProvider(ISccmInfoProvider sccmInfoProvider, ILog logger)
        {
            _sccmInfoProvider = sccmInfoProvider;
            _logger = logger;
        }
        
        public IEnumerable<Application> FindApplication(string applicationName, string applicationVersion)
        {
            var trimmedApplicationName = applicationName.Trim();
            var trimmedApplicationVersion = applicationVersion.Trim();

            var wmiQuery = $"SELECT * FROM SMS_Application WHERE IsLatest = 1 AND LocalizedDisplayName='{trimmedApplicationName}' AND SoftwareVersion='{trimmedApplicationVersion}'"; 

            var queryProcessor = this.SccmConnection.QueryProcessor;

            var applicationResults = queryProcessor.ExecuteQuery(wmiQuery) as WqlQueryResultsObject;

            if (applicationResults != null)
            {
                foreach (WqlResultObject appReference in applicationResults)
                {
                    if (appReference != null)
                    {
                        appReference.Get(); //Get all properties
                        var applicationFactory = new ApplicationFactory();
                        var applicationWrapper =
                            AppManWrapper.WrapExisting(appReference, applicationFactory) as AppManWrapper;
                        if (applicationWrapper != null)
                        {
                            var application = applicationWrapper.InnerAppManObject as Application;
                            if (application != null)
                            {
                                yield return application;
                            }
                            else
                            {
                                _logger.Error($"Failed to wrap to sccm application '{applicationName}'-'{applicationVersion}'");
                            }
                        }
                        else
                        {
                            _logger.Error($"Failed to get AppManWrapper for sccm application '{applicationName}'-'{applicationVersion}'");
                        }
                    }
                    else
                    {
                        _logger.Error($"Appreference is null for sccm application '{applicationName}'-'{applicationVersion}'");
                    }
                }
            }
            else
            {
                _logger.Warn($"Application not found: '{applicationName}'-'{applicationVersion}'");
            }
        }


        private WqlConnectionManager SccmConnection
        {
            get
            {
                if (_sccmConnection == null)
                {
                    _sccmConnection = new WqlConnectionManager();
                    var sccmServer = _sccmInfoProvider.GetSiteServer();
                    _sccmConnection.Connect(sccmServer);                    
                }
                return _sccmConnection;
            }
        }
        private WqlConnectionManager _sccmConnection;

        public void Dispose()
        {
            _sccmConnection?.Dispose();
        }
    }
}