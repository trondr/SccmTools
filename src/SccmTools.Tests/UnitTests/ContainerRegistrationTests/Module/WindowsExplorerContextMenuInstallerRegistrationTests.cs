using NUnit.Framework;
using SccmTools.Library.Module.Common.Install;
using SccmTools.Tests.Common;

namespace SccmTools.Tests.UnitTests.ContainerRegistrationTests.Module
{
    [TestFixture(Category = "UnitTests")]
    public class WindowsExplorerContextMenuInstallerRegistrationTests
    {        
        [Test, RequiresSTA]
        public static void WindowsExplorerContextMenuInstallerRegistrationTest()
        {
            BootStrapperTestsHelper.CheckThatNumberOfResolvedServicesAre<IWindowsExplorerContextMenuInstaller>(1);
            BootStrapperTestsHelper.CheckThatResolvedServiceIsOfInstanceType<IWindowsExplorerContextMenuInstaller, WindowsExplorerContextMenuInstaller>();
            BootStrapperTestsHelper.CheckThatResolvedServiceHasSingletonLifeCycle<IWindowsExplorerContextMenuInstaller>();
        }

    }
}