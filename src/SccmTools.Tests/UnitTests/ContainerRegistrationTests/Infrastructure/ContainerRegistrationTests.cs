using Castle.Windsor;
using NUnit.Framework;
using SccmTools.Infrastructure;
using SccmTools.Tests.Common;

namespace SccmTools.Tests.UnitTests.ContainerRegistrationTests.Infrastructure
{
    [TestFixture(Category = "UnitTests")]
    public class ContainerRegistrationTests
    {
        [Test, RequiresSTA]
        public void ContainerRegistrationTest()
        {
            BootStrapperTestsHelper.CheckThatNumberOfResolvedServicesAre<IWindsorContainer>(1);
            BootStrapperTestsHelper.CheckThatResolvedServiceIsOfInstanceType<IWindsorContainer, WindsorContainer>();
            BootStrapperTestsHelper.CheckThatResolvedServiceHasSingletonLifeCycle<IWindsorContainer>();
            using (var bootStrapper = new BootStrapper())
            {
                var target = bootStrapper.Container.Resolve<IWindsorContainer>();
                Assert.AreEqual(bootStrapper.Container.GetHashCode(), target.GetHashCode(), string.Format("Instance of service '{0}' is not the same.", typeof(IWindsorContainer)));
            }
        }
    }
}