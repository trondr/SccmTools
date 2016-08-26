using NUnit.Framework;
using SccmTools.Library.Infrastructure;
using SccmTools.Tests.Common;

namespace SccmTools.Tests.UnitTests.ContainerRegistrationTests.Infrastructure
{
    [TestFixture(Category = "UnitTests")]
    public class TypeMapperRegistrationTests
    {
        [Test, RequiresSTA]
        public void TypeMapperRegistrationTest()
        {
            BootStrapperTestsHelper.CheckThatNumberOfResolvedServicesAre<ITypeMapper>(1);
            BootStrapperTestsHelper.CheckThatResolvedServiceIsOfInstanceType<ITypeMapper, TypeMapper>();
            BootStrapperTestsHelper.CheckThatResolvedServiceHasSingletonLifeCycle<ITypeMapper>();
        }
    }
}