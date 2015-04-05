using System.Collections;
using System.ComponentModel;
using System.Reflection;
using SccmTools.Library.Common.Install;

namespace SccmTools
{
    [RunInstaller(true)]
    public partial class CustomInstaller : System.Configuration.Install.Installer
    {
        public CustomInstaller()
        {
            InitializeComponent();
        }

        public override void Install(IDictionary stateSaver)
        {            
            Context.LogMessage("Adding SccmTools to File Explorer context menu...");
            new WindowsExplorerContextMenuInstaller().Install("SccmTools", "Create application from package defintion file...", Assembly.GetExecutingAssembly().Location, "CreateApplicationFromPackageDefinition /packageDefinitionFile=\"%1\"");
            Context.LogMessage("Finished adding SccmTools to File Explorer context menu.");
            base.Install(stateSaver);
        }

        public override void Uninstall(IDictionary savedState)
        {            
            Context.LogMessage("Removing SccmTools from File Explorer context menu...");
            new WindowsExplorerContextMenuInstaller().UnInstall("SccmTools");
            Context.LogMessage("Finished removing SccmTools from File Explorer context menu.");
            base.Uninstall(savedState);
        }        
    }
}
