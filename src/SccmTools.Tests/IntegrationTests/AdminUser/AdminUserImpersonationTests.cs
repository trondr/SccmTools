using System;
using Common.Logging;
using Common.Logging.Simple;
using NUnit.Framework;
using SccmTools.Tests.Common;

namespace SccmTools.Tests.IntegrationTests.AdminUser
{
    [TestFixture(Category = "AdminUserIntegrationTests")]
    public class AdminUserImpersonationTests
    {
        private ILog _logger;
        private string _adminUser;
        private string _adminGroup;

        [SetUp]
        public void SetUp()
        {
            _logger = new ConsoleOutLogger(GetType().Name, LogLevel.All, true, false, false, "yyyy-MM-dd HH:mm:ss");
            _adminUser = "jenny";
            _adminGroup = "SomeSystemOperators";            
        }

        [TearDown]
        public void TearDown()
        {

        }

        [Test]
        public void VerifyIsloggedInWithAdminUser()
        {
            _logger.Info("Verify that the test is running under security context of: " + _adminUser);
            var expected = Environment.UserDomainName + "\\" + _adminUser;
            var actual = SecurityHelper.GetCurrentWindowsIdentityName();
            Assert.AreEqual(expected, actual, string.Format("Admin user is not logged in. Admin user '{0}' must be created in Active Directory prior to running these tests, and  tests started in the security context of '{0}'", _adminUser));
        }

        [Test]
        public void VerifyIsMemberOfAdminGroup()
        {            
            _logger.Info("Verify that logged in user is member of admin group: " + _adminGroup);
            const bool expected = true;
            var actual = SecurityHelper.IsInRole(_adminGroup);
            Assert.AreEqual(expected, actual, string.Format("Admin user is not member of admin group. Admin user '{0}' must be added as member of admin group '{1}' prior to running these tests.", _adminUser, _adminGroup));
        }
    }
}
