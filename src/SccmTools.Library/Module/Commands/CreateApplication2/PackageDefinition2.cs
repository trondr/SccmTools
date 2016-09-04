using Microsoft.ConfigurationManagement.ApplicationManagement;

namespace SccmTools.Library.Module.Commands.CreateApplication2
{
    public class PackageDefinition2
    {
        public PackageDefinition2(
            string name, 
            string version, 
            string publisher, 
            string comment, 
            string language, 
            string installCommandLine, 
            string unInstallCommandLine, 
            Icon icon, 
            string[] msiProductCodes,             
            string[] files, 
            string contentDirectory)
        {
            Name = name;
            Version = version;
            Publisher = publisher;
            Comment = comment;
            Language = language;
            InstallCommandLine = installCommandLine;
            UnInstallCommandLine = unInstallCommandLine;
            MsiProductCodes = msiProductCodes;
            Icon = icon;
            Files = files;
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

        public string[] MsiProductCodes { get; }

        public string[] Files { get; }

        public string ContentDirectory { get; }
    }
}