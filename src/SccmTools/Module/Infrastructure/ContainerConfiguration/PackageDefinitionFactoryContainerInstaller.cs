using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using SccmTools.Infrastructure.ContainerExtensions;
using SccmTools.Library.Module.Commands.CreateApplication2;
using SccmTools.Library.Module.Services;

namespace SccmTools.Module.Infrastructure.ContainerConfiguration
{
    [InstallerPriority(InstallerPriorityAttribute.DefaultPriority)]
    public class PackageDefinitionFactoryContainerInstaller: IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IPackageDefinitionFactory>().AsFactory());
            container.Register(
                Component.For<IPackageDefinition>()
                    .ImplementedBy<PackageDefinition>()
                    .Named(typeof(PackageDefinition).Name)
                    .LifeStyle.Transient);
        }
    }
}
