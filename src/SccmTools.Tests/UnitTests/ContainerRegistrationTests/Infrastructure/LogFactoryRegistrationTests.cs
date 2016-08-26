using NUnit.Framework;
using SccmTools.Infrastructure.ContainerExtensions;
using SccmTools.Tests.Common;

namespace SccmTools.Tests.UnitTests.ContainerRegistrationTests.Infrastructure
{
    [TestFixture(Category = "UnitTests")]
    public class LogFactoryRegistrationTests
    {
        [Test, RequiresSTA]
        public void LogFactoryRegistrationTest()
        {
            BootStrapperTestsHelper.CheckThatNumberOfResolvedServicesAre<ILogFactory>(1);
            BootStrapperTestsHelper.CheckThatResolvedServiceIsOfInstanceType<ILogFactory, LogFactory>();
            BootStrapperTestsHelper.CheckThatResolvedServiceHasSingletonLifeCycle<ILogFactory>();
        }
    }
}