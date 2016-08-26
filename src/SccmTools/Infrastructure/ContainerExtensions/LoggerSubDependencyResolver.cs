using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Context;
using Common.Logging;

namespace SccmTools.Infrastructure.ContainerExtensions
{
    /// <summary>
    /// When added to the container LoggerSubDependencyResolver instructs the
    /// container to resolve a class specific logger when resolving the ILog
    /// interface. The container is instructed to use the sub dependecy resolver
    /// by adding the following line of code to the container installer:
    /// 
    /// container.Kernel.Resolver.AddSubResolver(new LoggerSubDependencyResolver());
    /// 
    /// </summary>
    /// <example>
    /// public class SomeClass
    /// {
    ///    private ILog _logger;    
    ///    //
    ///    //Constructor
    ///    //
    ///    public SomeClass(ILog logger)
    ///    {
    ///       _logger = logger; 
    ///
    ///        //If the container has been instructed to use the LoggerSubDependencyResolver,
    ///        //the _logger is now a "SomeClass"-logger, ie. a class specific logger.
    ///        //It is now possible to track and filter log messages by class.
    ///    }
    /// }
    /// </example>
    public class LoggerSubDependencyResolver : ISubDependencyResolver
    {
        /// <summary>
        /// Check if dependency is of type ILog
        /// </summary>
        /// <param name="context"></param>
        /// <param name="contextHandlerResolver"></param>
        /// <param name="model"></param>
        /// <param name="dependency"></param>
        /// <returns></returns>
        public bool CanResolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model, DependencyModel dependency)
        {
            return dependency.TargetType == typeof(ILog);
        }

        /// <summary>
        /// If dependency is of type ILog resolve a class specific logger based on the implementation class name.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="contextHandlerResolver"></param>
        /// <param name="model"></param>
        /// <param name="dependency"></param>
        /// <returns></returns>
        public object Resolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model, DependencyModel dependency)
        {
            if (CanResolve(context, contextHandlerResolver, model, dependency))
            {
                if (dependency.TargetType == typeof(ILog))
                {
                    return LogManager.GetLogger(model.Implementation.FullName);
                }
            }
            return null;
        }
    }
}