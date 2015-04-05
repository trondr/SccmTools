using System;
using System.IO;
using Common.Logging;
using Microsoft.ConfigurationManagement.ApplicationManagement;
using Microsoft.ConfigurationManagement.ApplicationManagement.Serialization;
using SccmTools.Library.Infrastructure;
using SccmTools.Library.Services;

namespace SccmTools.Library.Commands.CreateApplication
{
    public class CreateApplicationFromPackageDefinitionCommandProvider : CommandProvider, ICreateApplicationFromPackageDefinitionCommandProvider
    {
        private readonly IProductCodeProvider _productCodeProvider;
        private readonly ISccmInfoProvider _sccmInfoProvider;
        private readonly ISccmApplication _sccmApplication;
        private readonly IPackageDefinitionFactory _packageDefinitionFactory;
        private readonly ILog _logger;

        public CreateApplicationFromPackageDefinitionCommandProvider(IProductCodeProvider productCodeProvider, ISccmInfoProvider sccmInfoProvider, ISccmApplication sccmApplication, IPackageDefinitionFactory packageDefinitionFactory, ILog logger)
        {
            _productCodeProvider = productCodeProvider;
            _sccmInfoProvider = sccmInfoProvider;
            _sccmApplication = sccmApplication;
            _packageDefinitionFactory = packageDefinitionFactory;
            _logger = logger;
        }

        public int CreateApplicationFromPackageDefinition(string packageDefinitionFile)
        {
            packageDefinitionFile = Environment.ExpandEnvironmentVariables(packageDefinitionFile);
            if (!File.Exists(packageDefinitionFile)) throw new FileNotFoundException("Package definition file not found.", packageDefinitionFile);
            var packageDefinitionUri = new Uri(packageDefinitionFile);
            if (!packageDefinitionUri.IsUnc) throw new SccmToolsException("Package definition file path is not an UNC path: " + packageDefinitionFile);
            _logger.InfoFormat("Creating application from package definition file '{0}'...", packageDefinitionFile);
            var packageDefinition = _packageDefinitionFactory.GetPackageDefinition(packageDefinitionFile);
            var returnValue = 0;

            NamedObject.DefaultScope = _sccmInfoProvider.GetScopeId();
            _logger.Info("Creating application object...");
            var appId = new ObjectId(_sccmInfoProvider.GetAuthoringScopeId(), "Application_" + Guid.NewGuid());
            var application = new Application(appId)
            {
                Title = packageDefinition.Name,
                SoftwareVersion = packageDefinition.Version,
                Version = 1,
                Description = packageDefinition.Comment,
                Publisher = packageDefinition.Publisher
            };
            _logger.Info("Creating application catalog info...");
            var appDisplayInfo = new AppDisplayInfo
            {
                Language = "en-US",
                Title = application.Title + " " + packageDefinition.Version,
                Publisher = packageDefinition.Publisher,
                Description = packageDefinition.Comment,
                Icon = packageDefinition.Icon
            };
            application.DisplayInfo.Add(appDisplayInfo);
            application.DisplayInfo.DefaultLanguage = appDisplayInfo.Language;
            application.Validate();
            _logger.Info("Creating application content...");
            var contentDirectory = new FileInfo(packageDefinitionFile).Directory;
            if (contentDirectory == null) { throw new SccmToolsException("Failed to derive parent directory for package definition file: " + packageDefinitionFile); }
            var content = ContentImporter.CreateContentFromFolder(contentDirectory.FullName);
            content.OnFastNetwork = ContentHandlingMode.Download;
            content.OnSlowNetwork = ContentHandlingMode.Download;
            content.FallbackToUnprotectedDP = false;

            _logger.Info("Creating application script installer with msi product code detection method...");
            var productCode = _productCodeProvider.GetProductCodeFromText(packageDefinition.Comment);
            if (string.IsNullOrWhiteSpace(productCode))
            {
                throw new SccmToolsException("ProductCode was not found in package definition file comment.");
            }
            var scriptInstaller = new ScriptInstaller
            {
                InstallCommandLine = packageDefinition.InstallCommandLine,
                UninstallCommandLine = packageDefinition.UnInstallCommandLine,
                DetectionMethod = DetectionMethod.ProductCode,
                ProductCode = productCode,
                InstallContent = new ContentRef(content)
            };
            scriptInstaller.Contents.Add(content);
            _logger.Info("Creating application deployment method...");
            var deploymentType = new DeploymentType(scriptInstaller, ScriptInstaller.TechnologyId, NativeHostingTechnology.TechnologyId)
            {
                Title = "Custom-Script-Installer-MSI",
            };
            application.DeploymentTypes.Add(deploymentType);
            _logger.Info("Saving application to SCCM...");
            var appDefintionXml = SccmSerializer.SerializeToString(application);
            if (_logger.IsDebugEnabled) _logger.DebugFormat("Application definition XML:{0}{1}", Environment.NewLine, appDefintionXml);
            _sccmApplication.Save(appDefintionXml);
            _logger.Info("Finished saving application to SCCM.");

            return returnValue;
        }
    }
}