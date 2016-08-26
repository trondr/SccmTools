using System;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Common.Logging;
using SccmTools.Infrastructure.ContainerExtensions;

namespace SccmTools.Infrastructure
{
    public sealed class BootStrapper : IDisposable
    {
        private IWindsorContainer _container;
        private readonly object _synch = new object();
        private bool _disposed;

        private ILog Logger
        {
            get
            {
                if (_logger == null)
                {
                    lock (_synch)
                    {
                        if (_logger == null)
                        {
                            _logger = LogManager.GetLogger(this.GetType().FullName);
                        }
                    }
                }
                return _logger;
            }
        }
        private ILog _logger;
        
        public IWindsorContainer Container
        {
            get
            {
                if (_container == null)
                {
                    lock(_synch)
                    {
                        if(_container == null)
                        {
                            _container = new WindsorContainer();
                            if (Logger.IsDebugEnabled)
                            {
                                _container.Kernel.HandlerRegistered += Kernel_HandlerRegistered;                                
                                _container.Kernel.DependencyResolving += Kernel_DependencyResolving;
                            }
                            _container.Install(FromAssembly.InThisApplication(new ContainerInstallerFactory()));
                        }
                    }                
                }
                return _container;
            }
        }
        
        private void Kernel_DependencyResolving(Castle.Core.ComponentModel client, Castle.Core.DependencyModel model, object dependency)
        {
            Logger.DebugFormat("IOC Container: Kernel dependency resolving. Client: {0}, Dependency key: {1}, dependency: {2}", client.Name, model.DependencyKey, dependency);
        }
        
        private void Kernel_HandlerRegistered(Castle.MicroKernel.IHandler handler, ref bool stateChanged)
        {
            //Logger.DebugFormat("IOC Container: Handler registred. Handler: {0}, State Changed: {1}", handler.ComponentModel.Name, stateChanged);
            if (!Logger.IsDebugEnabled) return;
            foreach (var service in handler.ComponentModel.Services)
            {
                Logger.DebugFormat("\tRegistered {0}/{1} ({2}) ({3})", service.FullName, handler.ComponentModel.Implementation.FullName, handler.ComponentModel.Name, handler.ComponentModel.LifestyleType);
            }
        }

        public void Dispose()
        {            
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (!disposing) return;
            _container?.Dispose();
            _container = null;
            _disposed = true;
        }
    }
}
