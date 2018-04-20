using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using SccmTools.Library.Module.Common.Install;

namespace SccmTools.Module
{
    [RunInstaller(true)]
    public partial class CustomInstaller : System.Configuration.Install.Installer
    {
        public CustomInstaller()
        {
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolver.ResolveHandler;
            InitializeComponent();
        }

        public override void Install(IDictionary stateSaver)
        {            
            Context.LogMessage("Adding SccmTools to File Explorer context menu...");
            new WindowsExplorerContextMenuInstaller().Install("SccmTools", "SccmTools - Create application from Definition", Assembly.GetExecutingAssembly().Location, "CreateApplicationFromDefinition /packageDefinitionFile=\"%1\"");
            Context.LogMessage("Finished adding SccmTools to File Explorer context menu.");
            
            Context.LogMessage("Adding SccmTools to Configuration Manager Console context menu...");
            new ConfigurationManagerContextMenuInstaller().Install("d2e2cba7-98f5-4d3b-bc2f-b670f0621207","SccmTools - Create Application from Definition...", Assembly.GetExecutingAssembly().Location, "CreateApplicationFromDefinition", Context);
            Context.LogMessage("Finished adding SccmTools to Configuration Manager Console context menu.");
            base.Install(stateSaver);
        }

        public override void Uninstall(IDictionary savedState)
        {            
            Context.LogMessage("Removing SccmTools from File Explorer context menu...");
            new WindowsExplorerContextMenuInstaller().UnInstall("SccmTools");
            Context.LogMessage("Finished removing SccmTools from File Explorer context menu.");
            Context.LogMessage("Removing SccmTools from Configuration Manager Console context menu...");
            new ConfigurationManagerContextMenuInstaller().UnInstall("d2e2cba7-98f5-4d3b-bc2f-b670f0621207","SccmTools - Create Application from Definition", Context);
            Context.LogMessage("Finished removing SccmTools from Configuration Manager Console context menu.");
            base.Uninstall(savedState);
        }        
    }
}
