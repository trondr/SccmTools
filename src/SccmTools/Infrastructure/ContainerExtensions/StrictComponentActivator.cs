using System;
using Castle.Core;
using Castle.DynamicProxy;
using Castle.MicroKernel;
using Castle.MicroKernel.ComponentActivator;
using Castle.MicroKernel.Context;

namespace SccmTools.Infrastructure.ContainerExtensions
{
    /// <summary>
    /// Make the activator of components throw excetions when errors occurs on resolving property dependencies
    /// </summary>
    public class StrictComponentActivator : DefaultComponentActivator
    {
        public StrictComponentActivator(ComponentModel model, IKernelInternal kernel,
            ComponentInstanceDelegate onCreation,
            ComponentInstanceDelegate onDestruction)
            : base(model, kernel, onCreation, onDestruction)
        {
        }

        /// <summary>
        /// Source: http://www.symbolsource.org/Public/Metadata/NuGet/Project/Castle.Windsor/3.0.0.3001/Release/.NETFramework,Version%3Dv4.0,Profile%3DClient/Castle.Windsor/Castle.Windsor/MicroKernel/ComponentActivator/DefaultComponentActivator.cs?ImageName=Castle.Windsor
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="context"></param>
        protected override void SetUpProperties(object instance, CreationContext context)
        {
            instance = ProxyUtil.GetUnproxiedInstance(instance);
            var resolver = Kernel.Resolver;
            foreach (var property in Model.Properties)
            {
                var value = ObtainPropertyValue(context, property, resolver);
                if (value == null)
                {
                    continue;
                }

                var setMethod = property.Property.GetSetMethod();
                try
                {
                    setMethod.Invoke(instance, new[] { value });
                }
                catch (Exception ex)
                {
                    var message =
                        String.Format(
                            "Error setting property {1}.{0} in component {2}. See inner exception for more information. If you don't want Windsor to set this property you can do it by either decorating it with {3} or via registration API.",
                            property.Property.Name, instance.GetType().Name, Model.Name, typeof(DoNotWireAttribute).Name);
                    throw new ComponentActivatorException(message, ex, Model);
                }
            }
        }


        /// <summary>
        /// Source: http://www.symbolsource.org/Public/Metadata/NuGet/Project/Castle.Windsor/3.0.0.3001/Release/.NETFramework,Version%3Dv4.0,Profile%3DClient/Castle.Windsor/Castle.Windsor/MicroKernel/ComponentActivator/DefaultComponentActivator.cs?ImageName=Castle.Windsor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="property"></param>
        /// <param name="resolver"></param>
        /// <returns></returns>
        private object ObtainPropertyValue(CreationContext context, PropertySet property, IDependencyResolver resolver)
        {
            if (property.Dependency.IsOptional == false || resolver.CanResolve(context, context.Handler, Model, property.Dependency))
            {
                try
                {
                    return resolver.Resolve(context, context.Handler, Model, property.Dependency);
                }
                catch (Exception ex)
                {
                    //trondr: Do not silently ignore exceptions when resolving property depencies.                    
                    //if (property.Dependency.IsOptional == false)
                    //{
                    //    throw;
                    //}
                    //trondr: Rethrow the exception
                    throw new ComponentActivatorException("Failed to resolve property: " + property.Property.Name , ex, Model);
                }
            }
            return null;
        }
    }
}