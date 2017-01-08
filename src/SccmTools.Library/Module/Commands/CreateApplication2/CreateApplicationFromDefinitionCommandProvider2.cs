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
        private readonly IPackageDefinitionFactory2 _packageDefinitionFactory;
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

            _logger.Info("Creating application script installer with msi product code detection method...");
            var installer = new ScriptInstaller()
            {
                InstallCommandLine = packageDefinition.InstallCommandLine,
                UninstallCommandLine = packageDefinition.UnInstallCommandLine,
                DetectionMethod = DetectionMethod.Enhanced,                
                InstallContent = new ContentRef(content)
            };
            installer.Contents.Add(content);

            var enhancedDetectionMethod = new EnhancedDetectionMethod();
            if (packageDefinition.Files.Length > 0)
            {
                var detectionMethodFile = packageDefinition.Files[0];
                var file = new FileInfo(detectionMethodFile.FileName);

                var detectionMethodDataType = GetDataTypeFromDectionMethodFile(detectionMethodFile);
                var detectionMethodData = GetDataFromDetectionMethodFile(detectionMethodFile);
                var constantValue = new ConstantValue(detectionMethodData, detectionMethodDataType);

                var fileSetting = new FileOrFolder(ConfigurationItemPartType.File, null);
                fileSetting.SettingDataType = detectionMethodDataType;
                fileSetting.FileOrFolderName = file.Name;
                fileSetting.Path = file.Directory.FullName;
                enhancedDetectionMethod.Settings.Add(fileSetting);

                var settingReference = new SettingReference(
                    application.Scope,
                    application.Name,
                    application.Version.GetValueOrDefault(),
                    fileSetting.LogicalName,
                    fileSetting.SettingDataType,
                    fileSetting.SourceType, false);

                settingReference.MethodType = ConfigurationItemSettingMethodType.Value;

                var operands = new CustomCollection<ExpressionBase>();
                operands.Add(settingReference);
                operands.Add(constantValue);
                var expressionOperator = GetExpressionOperator(detectionMethodFile);
                var expression = new Expression(expressionOperator,operands);
                var rule = new Rule("IsInstalledRule",NoncomplianceSeverity.Critical, null,expression);
                enhancedDetectionMethod.ChangeId();
                enhancedDetectionMethod.Rule = rule;
            }
            installer.EnhancedDetectionMethod = enhancedDetectionMethod;

            _logger.Info("Creating application deployment method...");
            var deploymentType = new DeploymentType(installer, MsiInstaller.TechnologyId, NativeHostingTechnology.TechnologyId)
            {
                Title = "Custom-Script-Installer-MSI",
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

        private ExpressionOperator GetExpressionOperator(DetectionMethodFile detectionMethodFile)
        {
            switch (detectionMethodFile.RuleOperator)
            {
                case RuleOperator.Equals:
                    return ExpressionOperator.IsEquals;                    
                case RuleOperator.NotEqualTo:
                    return ExpressionOperator.NotEquals;                    
                case RuleOperator.GreaterThanOrEqualTo:
                    return ExpressionOperator.GreaterEquals;
                case RuleOperator.GreaterThan:
                    return ExpressionOperator.GreaterThan;                    
                case RuleOperator.LessThan:
                    return ExpressionOperator.LessThan;
                case RuleOperator.LessThanOrEqualTo:
                    return ExpressionOperator.LessEquals;                    
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private string GetDataFromDetectionMethodFile(DetectionMethodFile detectionMethodFile)
        {
            switch (detectionMethodFile.RuleType)
            {
                case FileRuleType.Unknown:
                    throw new FileRuleTypeNotSpecifiedException();
                case FileRuleType.Exists:
                    return string.Empty;
                case FileRuleType.DateModified:
                    return detectionMethodFile.Modified.ToString();
                case FileRuleType.DateCreated:
                    return detectionMethodFile.Created.ToString();
                case FileRuleType.Version:
                    return detectionMethodFile.FileVersion.ToString();
                case FileRuleType.Size:
                    return detectionMethodFile.SizeInBytes.ToString();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private DataType GetDataTypeFromDectionMethodFile(DetectionMethodFile detectionMethodFile)
        {
            switch (detectionMethodFile.RuleType)
            {
                case FileRuleType.Unknown:
                    throw new FileRuleTypeNotSpecifiedException();
                case FileRuleType.Exists:
                    return DataType.Boolean;
                case FileRuleType.DateModified:
                    return DataType.DateTime;
                case FileRuleType.DateCreated:
                    return DataType.DateTime;
                case FileRuleType.Version:
                    return DataType.Version;
                case FileRuleType.Size:
                    return DataType.Int64;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }        
    }

    public class FileRuleTypeNotSpecifiedException : Exception
    {

    }
}