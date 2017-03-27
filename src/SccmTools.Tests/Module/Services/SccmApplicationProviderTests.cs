using System.Linq;
using Common.Logging;
using Common.Logging.Simple;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using SccmTools.Library.Module.Common.Wmi;
using SccmTools.Library.Module.Services;

namespace SccmTools.Tests.Module.Services
{
    [TestFixture(Category = "Manual")]
    public class SccmApplicationProviderTests
    {
        [Test()]
        public void FindApplicationTest()
        {
            var logger = new ConsoleOutLogger(GetType().Name, LogLevel.All, true, false, false, "yyyy-MM-dd HH:mm:ss");
            var sccmInfoProvider = new SccmInfoProvider(new Wmi());

            var target = new SccmApplicationProvider(sccmInfoProvider,logger);
            var actual = target.FindApplication("Ito Client Support 1.0.17044.19", "1.0.17044.19");
            var actualList = actual.ToList();
            Assert.IsTrue(actualList.Count>0, "Test Application not found");
        }
    }
}