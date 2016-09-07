using System;
using System.IO;

namespace SccmTools.Tests.UnitTests.Module.Common.IO
{
    public class TestIniFile : IDisposable
    {
        public TestIniFile(string iniFileDataString)
        {
            _testIniFileName = Path.GetTempFileName();
            using (var sw = new StreamWriter(_testIniFileName))
            {
                sw.Write(iniFileDataString);
            }
        }

        public string TestIniFileName
        {
            get { return _testIniFileName; }
        }
        private readonly string _testIniFileName;

        public void Dispose()
        {
            if (File.Exists(_testIniFileName))
            {
                File.Delete(_testIniFileName);
            }
        }
    }
}