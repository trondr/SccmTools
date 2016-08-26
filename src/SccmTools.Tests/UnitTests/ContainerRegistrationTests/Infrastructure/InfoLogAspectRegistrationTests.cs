using NUnit.Framework;
using SccmTools.Infrastructure.ContainerExtensions;
using SccmTools.Tests.Common;

namespace SccmTools.Tests.UnitTests.ContainerRegistrationTests.Infrastructure
{
    [TestFixture(Category = "UnitTests")]
    public class InfoLogAspectRegistrationTests
    {
        [Test, RequiresSTA]
        public void InfoLogAspectRegistrationTest()
        {
            BootStrapperTestsHelper.CheckThatNumberOfResolvedServicesAre<InfoLogAspect>(1);
            BootStrapperTestsHelper.CheckThatResolvedServiceIsOfInstanceType<InfoLogAspect, InfoLogAspect>();
            BootStrapperTestsHelper.CheckThatResolvedServiceHasSingletonLifeCycle<InfoLogAspect>();
        }
    }
}