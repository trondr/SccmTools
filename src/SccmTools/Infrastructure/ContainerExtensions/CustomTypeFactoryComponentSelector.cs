using System.Reflection;
using Castle.Facilities.TypedFactory;

namespace SccmTools.Infrastructure.ContainerExtensions
{
    public class CustomTypeFactoryComponentSelector : DefaultTypedFactoryComponentSelector
    {
        protected override string GetComponentName(MethodInfo method, object[] arguments)
        {
            if (method.Name == "Create" && arguments.Length == 1 && arguments[0] is string)
            {
                return (string)arguments[0];
            }
            return base.GetComponentName(method, arguments);
        }
    }
}