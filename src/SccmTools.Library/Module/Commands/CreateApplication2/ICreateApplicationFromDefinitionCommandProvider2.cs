using Common.Logging;
using SccmTools.Library.Module.Commands.CreateApplication;

namespace SccmTools.Library.Module.Commands.CreateApplication2
{
    public interface ICreateApplicationFromDefinitionCommandProvider2
    {
        int CreateApplicationFromDefinition(string packageDefinitionFileName);
    }

    public class CreateApplicationFromDefinitionCommandProvider2 : ICreateApplicationFromDefinitionCommandProvider2
    {
        private readonly IPackageDefinitionFileProvider _packageDefinitionFileProvider;
        private readonly IPackageDefinitionFactory2 _packageDefinitionFactory;
        private readonly IPackageDefinitionProvider _packageDefinitionProvider;
        private readonly ILog _logger;

        public CreateApplicationFromDefinitionCommandProvider2(IPackageDefinitionFileProvider packageDefinitionFileProvider, IPackageDefinitionProvider packageDefinitionProvider ,ILog logger)
        {
            _packageDefinitionFileProvider = packageDefinitionFileProvider;            
            _packageDefinitionProvider = packageDefinitionProvider;
            _logger = logger;
        }

        public int CreateApplicationFromDefinition(string packageDefinitionFileName)
        {
            var packageDefinitionFile = _packageDefinitionFileProvider.GetPackageDefinitionFile(packageDefinitionFileName);
            _logger.InfoFormat("Creating application from package definition file '{0}'...", packageDefinitionFile.FileName);
            var packageDefinition = _packageDefinitionProvider.ReadPackageDefinition(packageDefinitionFile.FileName);
            

            return 0;
        }

    }
}