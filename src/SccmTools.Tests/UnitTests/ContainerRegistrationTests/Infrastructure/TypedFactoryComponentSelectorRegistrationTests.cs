using Castle.Facilities.TypedFactory;
using NUnit.Framework;
using SccmTools.Infrastructure;
using SccmTools.Infrastructure.ContainerExtensions;
using SccmTools.Tests.Common;

namespace SccmTools.Tests.UnitTests.ContainerRegistrationTests.Infrastructure
{
    [TestFixture(Category = "UnitTests")]
    public class TypedFactoryComponentSelectorRegistrationTests
    {
        [Test, RequiresSTA]
        public void TypedFactoryComponentSelectorRegistrationTest()
        {
            BootStrapperTestsHelper.CheckThatNumberOfResolvedServicesAre<ITypedFactoryComponentSelector>(3);
            BootStrapperTestsHelper.CheckThatResolvedServiceHasSingletonLifeCycle<ITypedFactoryComponentSelector>();
            using (var bootStrapper = new BootStrapper())
            {
                var target = bootStrapper.Container.ResolveAll<ITypedFactoryComponentSelector>();
                Assert.AreEqual(typeof(CustomTypeFactoryComponentSelector), target[2].GetType(), "The third ITypedFactoryComponentSelector instance was not of type CustomTypeFactoryComponentSelector");
            }
        }
    }
}