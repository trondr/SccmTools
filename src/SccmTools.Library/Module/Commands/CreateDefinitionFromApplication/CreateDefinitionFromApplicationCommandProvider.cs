using System;
using System.IO;
using System.Linq;
using Microsoft.ConfigurationManagement.ApplicationManagement;
using NCmdLiner;
using SccmTools.Library.Module.Commands.CreateApplication;
using SccmTools.Library.Module.Services;

namespace SccmTools.Library.Module.Commands.CreateDefinitionFromApplication
{
    public class CreateDefinitionFromApplicationCommandProvider : ICreateDefinitionFromApplicationCommandProvider
    {
        private readonly ISccmApplicationProvider _sccmApplicationProvider;
        private readonly IPackageDefinitionProvider _packageDefinitionProvider;

        public CreateDefinitionFromApplicationCommandProvider(ISccmApplicationProvider sccmApplicationProvider, IPackageDefinitionProvider packageDefinitionProvider)
        {
            _sccmApplicationProvider = sccmApplicationProvider;
            _packageDefinitionProvider = packageDefinitionProvider;
        }
        
        public Result<int> CreateDefinitionFromApplication(string applicationName, string applicationVersion)
        {
            var logger = typeof(CreateDefinitionFromApplicationCommandProvider).Logger();
            var configurationManagerConsoleIsInstalled = F.GetAdminConsoleBinPath().Match(path => true, exception =>
            {
                logger.Error(exception.Message);
                return false;
            });
            if (!configurationManagerConsoleIsInstalled)
                return Result.Ok(1);

            var applications = _sccmApplicationProvider.FindApplication(applicationName, applicationVersion).ToList();
            if (applications.Count == 0)
            {
                logger.Error($"Application '{applicationName}-{applicationVersion}' does not exists in SCCM");
                return Result.Ok(1);
            }
            if (applications.Count > 1)
            {
                logger.Error($"More than one  application '{applicationName}-{applicationVersion}' exist in SCCM. Duplicates are not supported.");
                return Result.Ok(1);
            }
            var application = applications[0];
            var deploymentTypes = application.DeploymentTypes;
            if (deploymentTypes.Count == 0)
            {
                logger.Error($"No deployment type found on application '{applicationName}-{applicationVersion}'.");
                return Result.Ok(1);
            }
            if (deploymentTypes.Count > 1)
            {
                logger.Error($"More than 1 deployment type found on application '{applicationName}-{applicationVersion}'. Only one deployment type is supported.");
                return Result.Ok(1);
            }
            var packageDefinition = GetPackageDefinitionFromDeploymentType(application);
            var result = WritePackageDefinition(packageDefinition);
            var exitCode = result.Match(definition => Result.Ok(0), Result.Fail<int>);
            return exitCode;
        }

        private LanguageExt.Result<PackageDefinition> WritePackageDefinition(LanguageExt.Result<PackageDefinition> packageDefinition)
        {
            return packageDefinition.Match(definition => WritePackageDefinition(definition), exception => packageDefinition);
        }

        private LanguageExt.Result<PackageDefinition> WritePackageDefinition(PackageDefinition packageDefinition)
        {
            if (packageDefinition == null) throw new ArgumentNullException(nameof(packageDefinition));
            try
            {
                var packageDefinitionFilePath = Path.Combine(packageDefinition.ContentDirectory, "PackageDefinition.sms");
                if(System.IO.File.Exists(packageDefinitionFilePath))
                    return new LanguageExt.Result<PackageDefinition>(new SccmToolsException($"Package definition file allready exists for application '{packageDefinition.Name}-{packageDefinition.Version}'"));
                _packageDefinitionProvider.WritePackageDefinition(packageDefinitionFilePath, packageDefinition);
                return new LanguageExt.Result<PackageDefinition>(packageDefinition);
            }
            catch (Exception e)
            {
                return new LanguageExt.Result<PackageDefinition>(new SccmToolsException($"Failed to write package definition to file due to {e.Message}", e));
            }
        }

        private LanguageExt.Result<PackageDefinition> GetPackageDefinitionFromDeploymentType(Application application)
        {
            try
            {
                var installer = application.DeploymentTypes[0].Installer;
                var scriptInstaller = (ScriptInstaller)installer;                
                var packageDefinition = new PackageDefinition(
                    application.Title,
                    application.SoftwareVersion,
                    application.Publisher,
                    application.Description,
                    application.DisplayInfo[0].Language,
                    scriptInstaller.InstallCommandLine,
                    scriptInstaller.UninstallCommandLine,
                    application.DisplayInfo[0].Icon,
                    string.Empty,
                    null,
                    false,
                    application.DeploymentTypes[0].Installer.Contents[0].Location,
                    new Dependency[] { });
                return new LanguageExt.Result<PackageDefinition>(packageDefinition);
            }
            catch (Exception e)
            {
                return new LanguageExt.Result<PackageDefinition>(new SccmToolsException($"Failed to get package definition from application due to {e.Message}",e));
            }
        }

        private string GetContentLocationPath(DeploymentType deploymentType)
        {
            var contentLocationPath = deploymentType.Installer.Contents[0].Location;
            return contentLocationPath;
        }
    }
}