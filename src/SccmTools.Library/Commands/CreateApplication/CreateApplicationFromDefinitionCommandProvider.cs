using System;
using System.IO;
using Common.Logging;
using Microsoft.ConfigurationManagement.ApplicationManagement;
using Microsoft.ConfigurationManagement.ApplicationManagement.Serialization;
using Microsoft.Win32;
using SccmTools.Library.Common.IO;
using SccmTools.Library.Infrastructure;
using SccmTools.Library.Services;

namespace SccmTools.Library.Commands.CreateApplication
{
    public class CreateApplicationFromDefinitionCommandProvider : CommandProvider, ICreateApplicationFromDefinitionCommandProvider
    {
        private readonly IProductCodeProvider _productCodeProvider;
        private readonly ISccmInfoProvider _sccmInfoProvider;
        private readonly ISccmApplication _sccmApplication;
        private readonly IPackageDefinitionFactory _packageDefinitionFactory;
        private readonly IPathOperation _pathOperation;
        private readonly ILog _logger;

        public CreateApplicationFromDefinitionCommandProvider(IProductCodeProvider productCodeProvider, ISccmInfoProvider sccmInfoProvider, ISccmApplication sccmApplication, IPackageDefinitionFactory packageDefinitionFactory, IPathOperation pathOperation, ILog logger)
        {
            _productCodeProvider = productCodeProvider;
            _sccmInfoProvider = sccmInfoProvider;
            _sccmApplication = sccmApplication;
            _packageDefinitionFactory = packageDefinitionFactory;
            _pathOperation = pathOperation;
            _logger = logger;
        }

        public int CreateApplicationFromDefinition(string packageDefinitionFile)
        {
            if(string.IsNullOrWhiteSpace(packageDefinitionFile))
            {
                var openFileDialog = new OpenFileDialog
                {
                    Filter = string.Format("Package Definition (*.sms)|*.sms"),
                    Multiselect = false
                };
                var ok = openFileDialog.ShowDialog();
                if(ok == true)
                {
                    packageDefinitionFile = openFileDialog.FileName;
                }
                else
                {
                    return 1;
                }
            }
            packageDefinitionFile = Environment.ExpandEnvironmentVariables(packageDefinitionFile);
            if (!File.Exists(packageDefinitionFile)) throw new FileNotFoundException("Package definition file not found.", packageDefinitionFile);
            packageDefinitionFile = _pathOperation.GetUncPath(packageDefinitionFile);
            var packageDefinitionUri = new Uri(packageDefinitionFile);
            if (!packageDefinitionUri.IsUnc)
            { 
                throw new SccmToolsException(string.Format("Package definition file path '{0}' is not a network path. The path is not a UNC path or network drive that can be resolve to an UNC path.", packageDefinitionFile));
            }
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
            if(string.IsNullOrWhiteSpace(productCode))
            {
                productCode = _productCodeProvider.GetProductCodeFromMsiFileSearch(contentDirectory.FullName);
            }
            if (string.IsNullOrWhiteSpace(productCode))
            {
                throw new SccmToolsException(string.Format("ProductCode was not found any where in the [Package Definition]Comment nor from automatically searching for a msi file and its product code in content directory '{0}'. If there is more than one msi file in the content directory and its sub folders, the product code must be provided any where in the [Package Definition]Comment value.", contentDirectory.FullName));
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