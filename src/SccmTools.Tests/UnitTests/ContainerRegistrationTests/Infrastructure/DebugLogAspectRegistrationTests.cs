using NUnit.Framework;
using SccmTools.Infrastructure.ContainerExtensions;
using SccmTools.Tests.Common;

namespace SccmTools.Tests.UnitTests.ContainerRegistrationTests.Infrastructure
{
    [TestFixture(Category = "UnitTests")]
    public class DebugLogAspectRegistrationTests
    {
        [Test, RequiresSTA]
        public void DebugLogAspectRegistrationTest()
        {
            BootStrapperTestsHelper.CheckThatNumberOfResolvedServicesAre<DebugLogAspect>(1);
            BootStrapperTestsHelper.CheckThatResolvedServiceIsOfInstanceType<DebugLogAspect, DebugLogAspect>();
            BootStrapperTestsHelper.CheckThatResolvedServiceHasSingletonLifeCycle<DebugLogAspect>();
        }
    }
}