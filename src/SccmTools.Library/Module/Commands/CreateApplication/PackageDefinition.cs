using System;
using System.Collections.Generic;
using Microsoft.ConfigurationManagement.ApplicationManagement;

namespace SccmTools.Library.Module.Commands.CreateApplication
{
    public class PackageDefinition
    {
        private IEnumerable<Dependency> _dependencies;

        public PackageDefinition(
            string name, 
            string version, 
            string publisher, 
            string comment, 
            string language, 
            string installCommandLine, 
            string unInstallCommandLine, 
            Icon icon, 
            string msiProductCode,
            RegistryValue registryValue, 
            bool registryValueIs64Bit,
            string contentDirectory,
            IEnumerable<Dependency> dependencies             
            )
        {
            if (dependencies == null) throw new ArgumentNullException(nameof(dependencies));
            Name = name;
            Version = version;
            Publisher = publisher;
            Comment = comment;
            Language = language;
            InstallCommandLine = installCommandLine;
            UnInstallCommandLine = unInstallCommandLine;
            Icon = icon;
            MsiProductCode = msiProductCode;
            RegistryValue = registryValue;
            RegistryValueIs64Bit = registryValueIs64Bit;
            ContentDirectory = contentDirectory;
            _dependencies = dependencies;
        }

        public string Name { get; }

        public string Version { get; }

        public string Publisher { get; }

        public string Comment { get; }

        public string Language { get; }

        public string InstallCommandLine { get; }

        public string UnInstallCommandLine { get; }

        public Icon Icon { get; }

        public string MsiProductCode { get; }

        public RegistryValue RegistryValue { get; }

        public bool RegistryValueIs64Bit { get; }

        public string ContentDirectory { get; }

        public IEnumerable<Dependency> Dependencies
        {
            get
            {
                return _dependencies ?? (_dependencies = new List<Dependency>());
            }            
        }
    }
}