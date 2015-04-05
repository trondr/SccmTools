using System;
using System.IO;
using Castle.Core;
using Castle.Core.Internal;
using Castle.Facilities.Logging;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Common.Logging;
using NCmdLiner;
using SccmTools.Library.Common.Caching;
using SccmTools.Library.Infrastructure;
using SccmTools.Library.Services;
using SccmTools.Library.ViewModels;
using SccmTools.Library.Views;
using SingletonAttribute = SccmTools.Library.Infrastructure.SingletonAttribute;

namespace SccmTools.Infrastructure
{
    public class ContainerInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IWindsorContainer>().Instance(container));
            container.AddFacility<TypedFactoryFacility>();
            container.Register(Component.For<ITypedFactoryComponentSelector>().ImplementedBy<CustomTypeFactoryComponentSelector>());
            
            //Configure logging
            ILoggingConfiguration loggingConfiguration = new LoggingConfiguration();
            log4net.GlobalContext.Properties["LogFile"] = Path.Combine(loggingConfiguration.LogDirectoryPath, loggingConfiguration.LogFileName);
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile));
            
            var applicationRootNameSpace = typeof (Program).Namespace;

            container.AddFacility<LoggingFacility>(f => f.UseLog4Net().ConfiguredExternally());
            container.Kernel.Register(Component.For<ILog>().Instance(LogManager.GetLogger(applicationRootNameSpace))); //Default logger
            container.Kernel.Resolver.AddSubResolver(new LoggerSubDependencyResolver()); //Enable injection of class specific loggers
            
            //Manual registrations
            container.Register(Component.For<MainWindow>().Activator<StrictComponentActivator>());
            container.Register(Component.For<MainView>().Activator<StrictComponentActivator>());
            container.Register(Component.For<MainViewModel>().Activator<StrictComponentActivator>());

            //Factory registrations
            container.Register(Component.For<IPackageDefinitionFactory>().AsFactory());
            container.Register(
                Component.For<IPackageDefinition>()
                    .ImplementedBy<PackageDefinition>()
                    .Named(typeof(PackageDefinition).Name)
                    .LifeStyle.Transient);

            container.Register(Component.For<IInvocationLogStringBuilder>().ImplementedBy<InvocationLogStringBuilder>().LifestyleSingleton());
            container.Register(Component.For<ILogFactory>().ImplementedBy<LogFactory>().LifestyleSingleton());
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
                .WithService.DefaultInterfaces().LifestyleTransient()                
            );

            //
            //   Register caching interceptor
            //
            container.Register(Component.For<ICacheProvider>().ImplementedBy<CacheProvider>().LifeStyle.Singleton);
            
            //
            //  Register interfaces that should be cached
            //
            container.Register(Component.For<ISccmInfoProvider>()
                .ImplementedBy<SccmInfoProvider>()
                .LifeStyle.Singleton
                .Interceptors(new InterceptorReference(typeof(CacheAspect))).Anywhere);
            
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
                .WithService.DefaultInterfaces().LifestyleSingleton());
            //
            //   Register all transients found in the library
            //
            container.Register(Classes.FromAssemblyContaining<CommandDefinition>()
                .InNamespace(libraryRootNameSpace, true)
                .WithService.DefaultInterfaces().LifestyleTransient());
            
            IApplicationInfo applicationInfo = new ApplicationInfo();
            container.Register(Component.For<IApplicationInfo>().Instance(applicationInfo).LifestyleSingleton());
        }
    }
}
