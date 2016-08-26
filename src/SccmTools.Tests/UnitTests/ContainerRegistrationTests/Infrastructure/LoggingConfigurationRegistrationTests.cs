using NUnit.Framework;
using SccmTools.Library.Infrastructure;
using SccmTools.Tests.Common;

namespace SccmTools.Tests.UnitTests.ContainerRegistrationTests.Infrastructure
{
    [TestFixture(Category = "UnitTests")]
    public class LoggingConfigurationRegistrationTests
    {
        [Test, RequiresSTA]
        public void LoggingConfigurationRegistrationTest()
        {
            BootStrapperTestsHelper.CheckThatNumberOfResolvedServicesAre<ILoggingConfiguration>(1);
            BootStrapperTestsHelper.CheckThatResolvedServiceIsOfInstanceType<ILoggingConfiguration, LoggingConfiguration>();
            BootStrapperTestsHelper.CheckThatResolvedServiceHasSingletonLifeCycle<ILoggingConfiguration>();
        }
    }
}