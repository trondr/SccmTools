using Castle.Windsor.Diagnostics.Extensions;
using NUnit.Framework;
using SccmTools.Library.Module.Common.IO;

namespace SccmTools.Tests.UnitTests.Module.Common.IO
{
    [TestFixture()]
    public class IniFileOperationTests
    {
        [Test()]
        public void ReadKeysTest()
        {
            string iniFileTestData = GetIniFileTestData();
            using (var testIniFile = new TestIniFile(iniFileTestData))
            {
                var target = new IniFileOperation();
                var values = target.ReadKeys(testIniFile.TestIniFileName, "Section1", @"file\d*");
                int expectedCount = 4;
                Assert.AreEqual(expectedCount, values.Length);
            }
        }

        [Test()]
        public void ReadTest()
        {
            string iniFileTestData = GetIniFileTestData();
            using (var testIniFile = new TestIniFile(iniFileTestData))
            {
                var target = new IniFileOperation();
                var actual = target.Read(testIniFile.TestIniFileName, "Section1", @"file1");
                string expectedValue = "Value1";
                Assert.AreEqual(expectedValue, actual);
            }
        }

        [Test()]
        public void WriteTest()
        {
            string iniFileTestData = GetIniFileTestData();
            using (var testIniFile = new TestIniFile(iniFileTestData))
            {
                var target = new IniFileOperation();
                target.Write(testIniFile.TestIniFileName, "Section2", "file1", "Value21");
                var actual = target.Read(testIniFile.TestIniFileName, "Section2", @"file1");
                string expectedValue = "Value21";
                Assert.AreEqual(expectedValue, actual);
            }
        }



        private string GetIniFileTestData()
        {
            const string iniFileTestData =
@"
[Section1]
MsiProductCode={guid0}
file=Value0
file1=Value1
file2=Value2
file3=Value3
MsiProductCode1={guid1}
MsiProductCode2={guid2}
";
            return iniFileTestData;
        }

    }
}