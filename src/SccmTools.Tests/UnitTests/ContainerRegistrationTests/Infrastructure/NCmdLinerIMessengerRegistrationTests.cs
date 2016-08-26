using NCmdLiner;
using NUnit.Framework;
using SccmTools.Infrastructure;
using SccmTools.Tests.Common;

namespace SccmTools.Tests.UnitTests.ContainerRegistrationTests.Infrastructure
{
    [TestFixture(Category = "UnitTests")]
    public class NCmdLinerIMessengerRegistrationTests
    {
        [Test, RequiresSTA]
        public void NCmdLinerIMessengerRegistrationTest()
        {
            BootStrapperTestsHelper.CheckThatNumberOfResolvedServicesAre<IMessenger>(1);
            BootStrapperTestsHelper.CheckThatResolvedServiceIsOfInstanceType<IMessenger, NotepadMessenger>();
            BootStrapperTestsHelper.CheckThatResolvedServiceHasSingletonLifeCycle<IMessenger>();
        }
    }
}