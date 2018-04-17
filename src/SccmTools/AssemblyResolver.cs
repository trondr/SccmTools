// File: AssemblyResolver.cs
// Project Name: NMultiTool.Console
// Project Home: https://github.com/trondr
// License: New BSD License (BSD) http://www.opensource.org/licenses/BSD-3-Clause
// Credits: See the Credit folder in this project
// Copyright © <github.com/trondr> 2013 
// All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using SccmTools.Library.Module;

namespace SccmTools
{
    /// <summary>
    /// 
    /// The assembly resolver enables deployment of an executable with all referenced dlls included as embedded 
    /// resources in the executable or assemblies in a custom folder different from the application folder. 
    /// To achieve loading assemblies from emebedded resource: 1. Include this AssemblyResolver.cs in the in your console 
    /// or windows project. 2. Create a 'Libs' folder in the project. 3. Copy the referenced dlls to the 
    /// 'Libs' folder and include the dlls in the project. 4. Set build action to 'Embedded Resource' for each dll.
    /// 5. Add a static constructor to your Program.cs as shown in the example below to activate the assembly resolver.
    /// 
    /// </summary>
    ///  
    /// <example>
    /// 
    /// <code>
    /// class Program
    /// {
    ///    #region Constructor
    ///    static Program()
    ///    {         
    ///       AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolver.ResolveHandler;
    ///    }
    ///    
    ///
    ///   [STAThread]
    ///   static void Main(string[] args)
    ///   {
    ///      //Your code
    ///   }
    /// }
    /// </code>
    /// #endregion
    /// 
    /// </example>
    public static class AssemblyResolver
    {
        public static Assembly ResolveHandler(object sender, ResolveEventArgs resolveEventArgs)
        {
            var assemblyName = new AssemblyName(resolveEventArgs.Name);
            DebugConsoleLogging($"Resolving {assemblyName}...");
            var assembly = LoadAssembly(assemblyName.Name);
            return assembly;
        }

        private static void DebugConsoleLogging(string message)
        {
#if DEBUG
            Console.WriteLine(message);
#endif
        }

        private static string ResourceName(string assemblyName)
        {
            if (string.IsNullOrWhiteSpace(assemblyName)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(assemblyName));
            var resourceNameSpace = typeof(Program).Namespace;
            var resourceName = $"{resourceNameSpace}.Libs.{assemblyName}.dll";
            return resourceName;
        }

        private static Assembly ResourceAssembly(string assemblyName)
        {
            if (string.IsNullOrWhiteSpace(assemblyName)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(assemblyName));
            var resourceAssembly = Assembly.GetEntryAssembly();
            if (resourceAssembly == null)
                DebugConsoleLogging("Failed to get entry assembly. Assembly.GetEntryAssembly() returned null.");
            return resourceAssembly;
        }
        
        private static Assembly LoadAssembly(string assemblyName)
        {
            var resourceAssembly = ResourceAssembly(assemblyName);
            var resourceName = ResourceName(assemblyName);
            var assembly = LoadAssemblyFromResource(resourceAssembly, resourceName) ?? LoadAssemblyFromSearchPath(assemblyName, SearchPaths());
            return assembly;
        }

        private static Assembly LoadAssemblyFromResource(Assembly resourceAssembly, string resourceName)
        {
            if (resourceAssembly == null) throw new ArgumentNullException(nameof(resourceAssembly));
            if (string.IsNullOrWhiteSpace(resourceName)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(resourceName));
            DebugConsoleLogging($"Trying to find and load assembly '{resourceName}' from embedded resource in assembly '{resourceAssembly.FullName}'...");
            var assemblyData = GetAssemblyData(resourceAssembly, resourceName);
            var assembly = assemblyData?.LoadAssemblyFromAssemblyData();
            return assembly;
        }

        private static Assembly LoadAssemblyFromSearchPath(string assemblyName, IEnumerable<string> searchPaths)
        {
            if (assemblyName == null) throw new ArgumentNullException(nameof(assemblyName));
            if (searchPaths == null) throw new ArgumentNullException(nameof(searchPaths));
            try
            {
                foreach (var searchPath in searchPaths)
                {
                    DebugConsoleLogging($"Trying to find and load assembly '{assemblyName}.dll' from search path '{searchPath}'...");
                    var assemblyPath = Path.Combine(searchPath, $"{assemblyName}.dll");
                    if (File.Exists(assemblyPath))
                        return Assembly.LoadFile(assemblyPath);
                    var assemblyPathAlt = Path.Combine(searchPath, $"{assemblyName}.exe");
                    if (File.Exists(assemblyPathAlt))
                        return Assembly.LoadFile(assemblyPathAlt);
                }                            
            }
            catch (Exception e)
            {
                DebugConsoleLogging($"Failed to find and load assembly '{assemblyName}' from search paths due to {e.Message}");
            }
            DebugConsoleLogging($"Failed to find and load assembly '{assemblyName}' from search paths.");
            return null;
        }
        
        private static Assembly LoadAssemblyFromAssemblyData(this byte[] assemblyData)
        {
            if (assemblyData == null) throw new ArgumentNullException(nameof(assemblyData));
            try
            {
                return Assembly.Load(assemblyData);
            }
            catch (Exception e)
            {
                DebugConsoleLogging($"Failed to load assembly from assembly data due to {e.Message}");
                return null;
            }            
        }

        private static byte[] GetAssemblyData(this Assembly resourceAssembly, string resourceName)
        {
            if (resourceAssembly == null) throw new ArgumentNullException(nameof(resourceAssembly));
            if (string.IsNullOrWhiteSpace(resourceName)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(resourceName));
            try
            {
                using (var dllStream = resourceAssembly.GetManifestResourceStream(resourceName))
                {
                    if (dllStream == null)
                    {
                        DebugConsoleLogging($"Failed to get assembly data '{resourceName}' from embedded resource in resource assembly '{resourceAssembly.FullName}'. Embedded resource does not exist.");
                        return null;
                    }
                    var assemblyData = new BinaryReader(dllStream).ReadBytes((int)dllStream.Length);
                    return assemblyData;
                }
            }
            catch (Exception e)
            {
                DebugConsoleLogging($"Failed to get assembly data '{resourceName}' from embedded resource in resource assembly '{resourceAssembly.FullName}' due to {e.Message}.");                
            }
            return null;
        }

        private static IEnumerable<string> SearchPaths()
        {
            var searchPath = F.GetAdminConsoleBinPath().Match(path => path.Value, exception =>
            {
                typeof(AssemblyResolver).Logger().Error(exception.Message);
                return string.Empty;
            });
            return string.IsNullOrWhiteSpace(searchPath) ? new string[] { } : new[] {searchPath};            
        }        
    }
}