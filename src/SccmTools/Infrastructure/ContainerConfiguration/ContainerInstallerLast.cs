using System;
using Castle.Core.Internal;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using NCmdLiner;
using SccmTools.Infrastructure.ContainerExtensions;
using SccmTools.Library.Infrastructure;
using SccmTools.Library.Infrastructure.LifeStyles;

namespace SccmTools.Infrastructure.ContainerConfiguration
{
    [InstallerPriority(int.MaxValue)]
    public class ContainerInstallerLast : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            ///////////////////////////////////////////////////////////////////
            //Automatic registrations
            ///////////////////////////////////////////////////////////////////
            //
            //   Register all interceptors
            //
            container.Register(Classes.FromAssemblyInThisApplication()
                .Pick().If(type => type.Name.EndsWith("Aspect")).LifestyleSingleton());
            //
            //   Register all command providers and attach logging interceptor
            //
            const string libraryRootNameSpace = "SccmTools.Library";
            container.Register(Classes.FromAssemblyContaining<CommandProvider>()
                .InNamespace(libraryRootNameSpace, true)
                .If(type => type.Is<CommandProvider>())
                .Configure(registration => registration.Interceptors(new[] { typeof(InfoLogAspect) }))
                .WithService.DefaultInterfaces().LifestyleSingleton()                
            );

            //
            //   Register all command definitions
            //
            container.Register(
                Classes.FromAssemblyInThisApplication()
                .BasedOn<CommandDefinition>()
                .WithServiceBase()
                );
            //
            //   Register all singletons found in the library
            //
            container.Register(Classes.FromAssemblyContaining<CommandDefinition>()
                .InNamespace(libraryRootNameSpace, true)
                .If(type => Attribute.IsDefined(type, typeof(SingletonAttribute)))
                .WithService.FirstInterface().LifestyleSingleton());

            //
            //   Register all trancients found in the library
            //
            container.Register(Classes.FromAssemblyContaining<CommandDefinition>()
                .InNamespace(libraryRootNameSpace, true)
                .If(type => Attribute.IsDefined(type, typeof(TrancientAttribute)))
                .WithService.FirstInterface().LifestyleTransient());

            //
            //   Register the rest with default singelton life cycle
            //
            container.Register(Classes.FromAssemblyContaining<CommandDefinition>()
                .InNamespace(libraryRootNameSpace, true)
                .WithService.DefaultInterfaces().LifestyleSingleton());
            
            //
            // Register IApplicationInfo instance
            //
            IApplicationInfo applicationInfo = new ApplicationInfo();
            container.Register(Component.For<IApplicationInfo>().Instance(applicationInfo).LifestyleSingleton());
        }
    }
}
