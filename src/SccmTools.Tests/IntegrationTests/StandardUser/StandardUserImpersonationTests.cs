using System;
using Common.Logging;
using Common.Logging.Simple;
using NUnit.Framework;
using SccmTools.Tests.Common;

namespace SccmTools.Tests.IntegrationTests.StandardUser
{
    [TestFixture(Category = "StandardUserIntegrationTests")]
    public class StandardUserImpersonationTests
    {
        private ILog _logger;
        private string _standardUser;
        private string _adminGroup;

        [SetUp]
        public void SetUp()
        {
            _logger = new ConsoleOutLogger(GetType().Name, LogLevel.All, true, false, false, "yyyy-MM-dd HH:mm:ss");
            _standardUser = "jim";
            _adminGroup = "SomeSystemOperators";
        }

        [TearDown]
        public void TearDown()
        {

        }

        [Test]
        public void VerifyIsloggedInWithStandardUser()
        {
            _logger.Info("Verify that the test is running under security context of: " + _standardUser);
            var expected = Environment.UserDomainName + "\\" + _standardUser;
            var actual = SecurityHelper.GetCurrentWindowsIdentityName();
            Assert.AreEqual(expected, actual, string.Format("Standard user is not logged in. Standard user '{0}' must be created in Active Directory prior to running these tests, and  tests started in the security context of '{0}'", _standardUser));
        }

        [Test]
        public void VerifyIsNotMemberOfAdminGroup()
        {
            _logger.Info("Verify that logged in user is NOT member of admin group: " + _adminGroup);
            const bool expected = false;
            var actual = SecurityHelper.IsInRole(_adminGroup);
            Assert.AreEqual(expected, actual, string.Format("Logged in user is member of admin group. Admin user '{0}' must be removed as member of admin group '{1}' prior to running these tests.", _standardUser, _adminGroup));
        }
    }
}
