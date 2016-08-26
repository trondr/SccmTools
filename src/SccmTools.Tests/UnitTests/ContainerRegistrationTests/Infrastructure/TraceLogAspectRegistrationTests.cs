using NUnit.Framework;
using SccmTools.Infrastructure.ContainerExtensions;
using SccmTools.Tests.Common;

namespace SccmTools.Tests.UnitTests.ContainerRegistrationTests.Infrastructure
{
    [TestFixture(Category = "UnitTests")]
    public class TraceLogAspectRegistrationTests
    {
        [Test, RequiresSTA]
        public void TraceLogAspectRegistrationTest()
        {
            BootStrapperTestsHelper.CheckThatNumberOfResolvedServicesAre<TraceLogAspect>(1);
            BootStrapperTestsHelper.CheckThatResolvedServiceIsOfInstanceType<TraceLogAspect, TraceLogAspect>();
            BootStrapperTestsHelper.CheckThatResolvedServiceHasSingletonLifeCycle<TraceLogAspect>();
        }
    }
}