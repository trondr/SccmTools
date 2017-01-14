using Microsoft.ConfigurationManagement.ApplicationManagement;

namespace SccmTools.Library.Module.Commands.CreateApplication
{
    public class PackageDefinition
    {
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
            string contentDirectory)
        {
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

        public bool RegistryValueIs64Bit { get; set; }

        public string ContentDirectory { get; }
    }
}