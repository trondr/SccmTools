using System;
using System.Collections.Generic;
using System.Management;
using System.Runtime.InteropServices;

namespace SccmTools.Library.Common.Wmi
{
    public static class WmiManagementObjectExtensions
    {
        public static ManagementBaseObject InvokeMethod(this ManagementObject managementObject, string methodName, IEnumerable<WmiParameter> parameters)
        {
            var methodParameters = GetMetodParameters(managementObject, methodName, parameters);
            var resultObject = managementObject.InvokeMethod(methodName,methodParameters, new InvokeMethodOptions());
            if(resultObject != null)
            {
                var returnValue = resultObject.GetPropertyValue("ReturnValue");
                var errorCode = Convert.ToUInt32(returnValue);
                if(errorCode != 0)
                { 
                    var ex = Marshal.GetExceptionForHR((int)returnValue);
                    throw new SccmToolsException(string.Format("Wmi method invocation failed for method '{0}'.", methodName), ex);
                }
            }
            if(resultObject == null)
            {
                throw new SccmToolsException(string.Format("Wmi method invocation failed for method '{0}'. Result object was null.", methodName));
            }
            return resultObject;
        }

        private static ManagementBaseObject GetMetodParameters(ManagementObject managementObject, string methodName, IEnumerable<WmiParameter> parameters)
        {
            if (managementObject == null) throw new ArgumentNullException("managementObject");
            if (parameters == null) throw new ArgumentNullException("parameters");

            var methodParameters = managementObject.GetMethodParameters(methodName);
            foreach (var parameter in parameters)
            {
                try
                {
                    methodParameters.SetPropertyValue(parameter.Name, parameter.Value);
                }
                catch (Exception ex)
                {
                    var msg = string.Format("Failed to set method parameter '{0}' and value '{1}' for for method '{2}'", parameter.Name, parameter.Value, methodName);
                    throw new SccmToolsException(msg, ex);
                }                
            }
            return methodParameters;
        }
    }
}