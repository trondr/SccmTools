using System;
using System.IO;
using Common.Logging;
using Microsoft.ConfigurationManagement.ApplicationManagement;
using Microsoft.ConfigurationManagement.ApplicationManagement.Serialization;
using Microsoft.ConfigurationManagement.DesiredConfigurationManagement;
using Microsoft.ConfigurationManagement.DesiredConfigurationManagement.ExpressionOperators;
using Microsoft.SystemsManagementServer.DesiredConfigurationManagement.Expressions;
using Microsoft.SystemsManagementServer.DesiredConfigurationManagement.Rules;
using SccmTools.Library.Module.Commands.CreateApplication;
using SccmTools.Library.Module.Services;

namespace SccmTools.Library.Module.Commands.CreateApplication2
{
    public class CreateApplicationFromDefinitionCommandProvider2 : ICreateApplicationFromDefinitionCommandProvider2
    {
        private readonly IPackageDefinitionFileProvider _packageDefinitionFileProvider;        
        private readonly IPackageDefinitionProvider _packageDefinitionProvider;
        private readonly ISccmApplication _sccmApplication;
        private readonly ISccmInfoProvider _sccmInfoProvider;
        private readonly ILog _logger;

        public CreateApplicationFromDefinitionCommandProvider2(
            IPackageDefinitionFileProvider packageDefinitionFileProvider, 
            IPackageDefinitionProvider packageDefinitionProvider,
            ISccmApplication sccmApplication, 
            ISccmInfoProvider sccmInfoProvider,
            ILog logger)
        {
            _packageDefinitionFileProvider = packageDefinitionFileProvider;            
            _packageDefinitionProvider = packageDefinitionProvider;
            _sccmApplication = sccmApplication;
            _sccmInfoProvider = sccmInfoProvider;
            _logger = logger;
        }

        public int CreateApplicationFromDefinition(string packageDefinitionFileName)
        {
            var packageDefinitionFile = _packageDefinitionFileProvider.GetPackageDefinitionFile(packageDefinitionFileName);
            _logger.InfoFormat("Creating application from package definition file '{0}'...", packageDefinitionFile.FileName);
            var packageDefinition = _packageDefinitionProvider.ReadPackageDefinition(packageDefinitionFile.FileName);
            
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
            var contentDirectory = new FileInfo(packageDefinitionFile.FileName).Directory;
            if (contentDirectory == null) { throw new SccmToolsException("Failed to derive parent directory for package definition file: " + packageDefinitionFile); }
            var content = ContentImporter.CreateContentFromFolder(contentDirectory.FullName);
            content.OnFastNetwork = ContentHandlingMode.Download;
            content.OnSlowNetwork = ContentHandlingMode.Download;
            content.FallbackToUnprotectedDP = false;

            _logger.Info("Creating application script installer...");
            var detectionMethod = GetDetectionMethodFromPackageDefinition(packageDefinition);
            var installer = new ScriptInstaller()
            {
                InstallCommandLine = packageDefinition.InstallCommandLine,
                UninstallCommandLine = packageDefinition.UnInstallCommandLine,
                DetectionMethod = detectionMethod,
                InstallContent = new ContentRef(content)
            };
            installer.Contents.Add(content);
            _logger.Info("Creating installer detection method...");
            if (detectionMethod == DetectionMethod.ProductCode)
            {
                installer.ProductCode = packageDefinition.MsiProductCode;
            }
            else if (detectionMethod == DetectionMethod.Enhanced)
            {
                installer.EnhancedDetectionMethod = GetRegistrySettingEnhancedDetectionMethod(packageDefinition, application);
            }
            
            _logger.Info("Creating application deployment method...");
            var technologyId = GetTechnologyId(packageDefinition);
            var deploymentType = new DeploymentType(installer, technologyId, NativeHostingTechnology.TechnologyId)
            {
                Title = GetDeploymentTypeTitle(detectionMethod),
                Version = 1,
            };
            application.DeploymentTypes.Add(deploymentType);
            _logger.Info("Saving application to SCCM...");
            var appDefintionXml = SccmSerializer.SerializeToString(application);
            if (_logger.IsDebugEnabled) _logger.DebugFormat("Application definition XML:{0}{1}", Environment.NewLine, appDefintionXml);
            _sccmApplication.Save(appDefintionXml);
            _logger.Info("Finished saving application to SCCM.");
            
            return 0;
        }

        private string GetDeploymentTypeTitle(DetectionMethod detectionMethod)
        {
            switch (detectionMethod)
            {
                case DetectionMethod.ProductCode:
                    return "Custom-Script-Installer-MSI";                    
                case DetectionMethod.Script:
                    return "Custom-Script-Installer-SCR";                    
                case DetectionMethod.Enhanced:
                    return "Custom-Script-Installer-REG";                    
                default:
                    throw new ArgumentOutOfRangeException(nameof(detectionMethod), detectionMethod, null);
            }
        }

        private string GetTechnologyId(PackageDefinition2 packageDefinition)
        {
            if(! string.IsNullOrWhiteSpace(packageDefinition.MsiProductCode))
                return MsiInstaller.TechnologyId;
            if(packageDefinition.RegistryValue != null)
                return ScriptInstaller.TechnologyId;
            throw new DetectionMethodNotSpecifiedException("Unable to determine installer technology id due to detection method has not been specied in the PackageDefinition.sms");
        }

        private EnhancedDetectionMethod GetRegistrySettingEnhancedDetectionMethod(PackageDefinition2 packageDefinition, Application application)
        {
            var enhancedDetectionMethod = new EnhancedDetectionMethod();
            var registrySetting = new RegistrySetting(null)
            {
                RootKey = packageDefinition.RegistryValue.RootKey,
                Key = packageDefinition.RegistryValue.Key,
                Is64Bit = packageDefinition.RegistryValueIs64Bit,
                ValueName = packageDefinition.RegistryValue.ValueName,
                CreateMissingPath = false,
                SettingDataType = DataType.String
            };
            enhancedDetectionMethod.Settings.Add(registrySetting);

            var expectedValue = new ConstantValue(packageDefinition.RegistryValue.Value, DataType.String);

            var settingReference = new SettingReference(
                application.Scope,
                application.Name,
                application.Version.GetValueOrDefault(),
                registrySetting.LogicalName,
                registrySetting.SettingDataType,
                registrySetting.SourceType,
                false
                );
            settingReference.MethodType = ConfigurationItemSettingMethodType.Value;

            var operands = new CustomCollection<ExpressionBase> {settingReference, expectedValue};

            var expression = new Expression(ExpressionOperator.IsEquals, operands);

            var rule = new Rule("IsInstalledRule", NoncomplianceSeverity.None, null, expression);

            enhancedDetectionMethod.Rule = rule;

            return enhancedDetectionMethod;
        }

        private DetectionMethod GetDetectionMethodFromPackageDefinition(PackageDefinition2 packageDefinition)
        {
            if(! string.IsNullOrWhiteSpace(packageDefinition.MsiProductCode))
                return DetectionMethod.ProductCode;
            if(packageDefinition.RegistryValue != null)
                return DetectionMethod.Enhanced;
            throw new DetectionMethodNotSpecifiedException("Unable to determine detection method type due to detection method has not been specied in the PackageDefinition.sms");
        }        
    }
}