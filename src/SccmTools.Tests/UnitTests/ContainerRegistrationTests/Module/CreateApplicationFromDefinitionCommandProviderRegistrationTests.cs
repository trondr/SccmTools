using NUnit.Framework;
using SccmTools.Library.Commands.CreateApplication;
using SccmTools.Tests.Common;

namespace SccmTools.Tests.UnitTests.ContainerRegistrationTests.Module
{
    [TestFixture(Category = "UnitTests")]
    public class CreateApplicationFromDefinitionCommandProviderRegistrationTests
    {        
        [Test, RequiresSTA]
        public static void ExampleCommandProviderRegistrationTest()
        {
            BootStrapperTestsHelper.CheckThatNumberOfResolvedServicesAre<ICreateApplicationFromDefinitionCommandProvider>(1);
            BootStrapperTestsHelper.CheckThatResolvedServiceIsOfInstanceTypeName<ICreateApplicationFromDefinitionCommandProvider>("ICreateApplicationFromDefinitionCommandProviderProxy");
            BootStrapperTestsHelper.CheckThatResolvedServiceHasSingletonLifeCycle<ICreateApplicationFromDefinitionCommandProvider>();
        }

    }
}