using System;
using System.IO;
using Castle.Core.Internal;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Common.Logging;
using NCmdLiner;
using SccmTools.Infrastructure.ContainerExtensions;
using SccmTools.Library.Infrastructure;

namespace SccmTools.Infrastructure.ContainerConfiguration
{
    [InstallerPriority(0)]
    public class ContainerInstallerFirst : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            //
            // Configure container
            //
            container.Register(Component.For<IWindsorContainer>().Instance(container));
            container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel));
            container.AddFacility<TypedFactoryFacility>();
            container.Register(Component.For<ITypedFactoryComponentSelector>().ImplementedBy<CustomTypeFactoryComponentSelector>());            
            //
            //   Configure logging
            //
            ILoggingConfiguration loggingConfiguration = new LoggingConfiguration();
            log4net.GlobalContext.Properties["LogFile"] = Path.Combine(loggingConfiguration.LogDirectoryPath, loggingConfiguration.LogFileName);
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile));
            var applicationRootNameSpace = typeof (Program).Namespace;
            container.Kernel.Register(Component.For<ILog>().Instance(LogManager.GetLogger(applicationRootNameSpace))); //Default logger
            container.Kernel.Resolver.AddSubResolver(new LoggerSubDependencyResolver()); //Enable injection of class specific loggers
            container.Register(Component.For<IInvocationLogStringBuilder>().ImplementedBy<InvocationLogStringBuilder>().LifestyleSingleton());
            container.Register(Component.For<ILogFactory>().ImplementedBy<LogFactory>().LifestyleSingleton()); 
            container.Register(Classes.FromAssemblyContaining<ITypeMapper>().IncludeNonPublicTypes().BasedOn<AutoMapper.Profile>().WithService.Base());   
            //
            //   Configure NCmdLiner
            //
            container.Register(Component.For<IMessenger>().ImplementedBy<NotepadMessenger>());
        }
    }
}
