using Castle.Windsor;
using Castle.Windsor.Installer;
using Common.Logging;

namespace SccmTools.Infrastructure
{
    public static class BootStrapper
    {
        public static object Synch = new object();
        public static IWindsorContainer Container
        {
            get
            {
                if (_container == null)
                {
                    lock (Synch)
                    {
                        if (_container == null)
                        {
                             _container = new WindsorContainer();
                            if (Logger.IsDebugEnabled)
                            {
                                _container.Kernel.HandlerRegistered += Kernel_HandlerRegistered;                                
                                _container.Kernel.DependencyResolving += Kernel_DependencyResolving;
                            }
                            _container.Install(FromAssembly.InThisApplication());
                        }
                    }
                }
                return _container;                
            }
        }

        private static ILog Logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = LogManager.GetLogger("Bootstrapper");
                }
                return _logger;
            }
        }
        private static ILog _logger;

        static void Kernel_DependencyResolving(Castle.Core.ComponentModel client, Castle.Core.DependencyModel model, object dependency)
        {
            Logger.DebugFormat("IOC Container: Kernel dependency resolving. Client: {0}, Dependency key: {1}, dependency: {2}", client.Name, model.DependencyKey, dependency);
        }
        private static IWindsorContainer _container;

        static void Kernel_HandlerRegistered(Castle.MicroKernel.IHandler handler, ref bool stateChanged)
        {
          
            Logger.DebugFormat("IOC Container: Handler registred. Handler: {0}, State Changed: {1}", handler.ComponentModel.Name, stateChanged);
            if (Logger.IsDebugEnabled)
                foreach (var service in handler.ComponentModel.Services)
                {
                    Logger.DebugFormat("\tRegistered {0}/{1} ({2}) ({3})", service.FullName, handler.ComponentModel.Implementation.FullName, handler.ComponentModel.Name, handler.ComponentModel.LifestyleType);
                }        
        }        
    }
}
