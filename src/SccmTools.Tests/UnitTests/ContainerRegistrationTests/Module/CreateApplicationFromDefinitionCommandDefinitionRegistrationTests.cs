using NUnit.Framework;
using SccmTools.Library.Infrastructure;
using SccmTools.Module.Commands;
using SccmTools.Tests.Common;

namespace SccmTools.Tests.UnitTests.ContainerRegistrationTests.Module
{
    [TestFixture(Category = "UnitTests")]
    public class CreateApplicationFromDefinitionCommandDefinitionRegistrationTests
    {        
        [Test, RequiresSTA]
        public static void CreateApplicationFromDefinitionCommandDefinitionRegistrationTest()
        {
            BootStrapperTestsHelper.CheckThatOneOfTheResolvedServicesAre<CommandDefinition, CreateApplicationFromDefinitionCommandDefinition>("Not registered: " + typeof(CreateApplicationFromDefinitionCommandDefinition).FullName);
        }        
    }
}