using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using SccmTools.Library.Module.Commands.CreateDefinitionFromApplication;

namespace SccmTools.Module.Infrastructure.ContainerConfiguration
{
    public class CreateDefinitionFromApplicationCommandProviderContainerInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<ICreateDefinitionFromApplicationCommandProviderFactory>().AsFactory());
            container.Register(
                Component
                    .For<ICreateDefinitionFromApplicationCommandProvider>()
                    .ImplementedBy<CreateDefinitionFromApplicationCommandProvider>()
                    .Named(nameof(CreateDefinitionFromApplicationCommandProvider))
                    .LifestyleTransient()
                );
        }
    }
}
