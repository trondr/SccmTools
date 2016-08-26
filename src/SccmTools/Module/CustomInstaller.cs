using System.Collections;
using System.ComponentModel;

namespace SccmTools.Module
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
            //Example: Adding a command to windows explorer contect menu
            //this.Context.LogMessage("Adding SccmTools to File Explorer context menu...");
            //new WindowsExplorerContextMenuInstaller().Install("SccmTools", "Create Something...", Assembly.GetExecutingAssembly().Location, "CreateSomething /exampleParameter=\"%1\"");
            //this.Context.LogMessage("Finnished adding SccmTools to File Explorer context menu.");
            
            base.Install(stateSaver);
        }

        public override void Uninstall(IDictionary savedState)
        {
            //Example: Removing previously installed command from windows explorer contect menu
            //this.Context.LogMessage("Removing SccmTools from File Explorer context menu...");
            //new WindowsExplorerContextMenuInstaller().UnInstall("SccmTools");
            //this.Context.LogMessage("Finished removing SccmTools from File Explorer context menu.");
            
            base.Uninstall(savedState);
        }        
    }
}
