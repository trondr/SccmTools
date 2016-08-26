using NUnit.Framework;
using SccmTools.Infrastructure.ContainerExtensions;
using SccmTools.Tests.Common;

namespace SccmTools.Tests.UnitTests.ContainerRegistrationTests.Infrastructure
{
    [TestFixture(Category = "UnitTests")]
    public class InvocationLogStringBuilderRegistrationTests
    {
        [Test, RequiresSTA]
        public void InvocationLogStringBuilderRegistrationTest()
        {
            BootStrapperTestsHelper.CheckThatNumberOfResolvedServicesAre<IInvocationLogStringBuilder>(1);
            BootStrapperTestsHelper.CheckThatResolvedServiceIsOfInstanceType<IInvocationLogStringBuilder, InvocationLogStringBuilder>();
        }
    }
}