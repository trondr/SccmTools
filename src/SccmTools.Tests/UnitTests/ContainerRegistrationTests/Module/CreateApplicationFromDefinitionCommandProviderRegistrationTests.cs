using NUnit.Framework;
using SccmTools.Library.Module.Commands.CreateApplication;
using SccmTools.Tests.Common;

namespace SccmTools.Tests.UnitTests.ContainerRegistrationTests.Module
{
    [TestFixture(Category = "UnitTests")]
    public class CreateApplicationFromDefinitionCommandProviderRegistrationTests
    {        
        [Test, RequiresSTA]
        public static void CreateApplicationFromDefinitionCommandProviderRegistrationTest()
        {
            BootStrapperTestsHelper.CheckThatNumberOfResolvedServicesAre<ICreateApplicationFromDefinitionCommandProvider>(1);
            BootStrapperTestsHelper.CheckThatResolvedServiceIsOfInstanceTypeName<ICreateApplicationFromDefinitionCommandProvider>("CreateApplicationFromDefinitionCommandProvider");
            BootStrapperTestsHelper.CheckThatResolvedServiceHasSingletonLifeCycle<ICreateApplicationFromDefinitionCommandProvider>();
        }

    }
}