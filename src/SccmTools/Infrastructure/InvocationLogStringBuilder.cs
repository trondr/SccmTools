using System;
using System.Linq;
using System.Text;
using Castle.DynamicProxy;
using Newtonsoft.Json;

namespace SccmTools.Infrastructure
{
    public class InvocationLogStringBuilder : IInvocationLogStringBuilder
    {
        public string BuildLogString(IInvocation invocation, InvocationPhase invocationPhase)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{0}: {1}.{2}(", invocationPhase, invocation.TargetType.Name, invocation.Method.Name);
            foreach (var argument in invocation.Arguments)
            {
                var argumentDescription = argument == null ? "null" : ObjectToString(argument);
                sb.Append(argumentDescription).Append(",");
            }
            if (invocation.Arguments.Any()) sb.Length--;
            sb.Append(")");
            if (invocation.ReturnValue != null)
            {
                sb.Append(", Returned: " + ObjectToString(invocation.ReturnValue));
            }
            return sb.ToString();
        }

        private static string ObjectToString(object obj)
        {
            var objtype = obj.GetType();
            if (objtype == typeof(String))
            {
                return string.Format("\"{0}\"", obj);
            }
            if (objtype.IsPrimitive || !objtype.IsClass)
            {
                return obj.ToString();
            }
            return JsonConvert.SerializeObject(obj);
        }
    }
}