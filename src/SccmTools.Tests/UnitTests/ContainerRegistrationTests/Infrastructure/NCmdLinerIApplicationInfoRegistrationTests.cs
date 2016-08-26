using NCmdLiner;
using NUnit.Framework;
using SccmTools.Tests.Common;

namespace SccmTools.Tests.UnitTests.ContainerRegistrationTests.Infrastructure
{
    [TestFixture(Category = "UnitTests")]
    public class NCmdLinerIApplicationInfoRegistrationTests
    {
        [Test, RequiresSTA]
        public void NCmdLinerIApplicationInfoRegistrationTest()
        {
            BootStrapperTestsHelper.CheckThatNumberOfResolvedServicesAre<IApplicationInfo>(1);
            BootStrapperTestsHelper.CheckThatResolvedServiceIsOfInstanceType<IApplicationInfo, ApplicationInfo>();
            BootStrapperTestsHelper.CheckThatResolvedServiceHasSingletonLifeCycle<IApplicationInfo>();
        }
    }
}