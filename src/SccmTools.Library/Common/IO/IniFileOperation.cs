using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using SccmTools.Library.Infrastructure;

namespace SccmTools.Library.Common.IO
{
    
    /// <summary>
    /// Ini file operation
    /// </summary>
    [Singleton]
    public class IniFileOperation : IIniFileOperation
    {        
        [DllImport("kernel32", SetLastError = true)]
        private static extern int WritePrivateProfileString(string section, string key,string val,string filePath);
        [DllImport("kernel32", SetLastError = true)]
        private static extern int GetPrivateProfileString(string section, string key,string def, StringBuilder retVal, int size,string filePath);

        public string Read(string path, string section, string key)
        {
            var value = new StringBuilder(255);
            GetPrivateProfileString(section, key, "", value, 255, path);
            var errorCode = Marshal.GetLastWin32Error();
            if(errorCode != 0)
            {
                throw new Win32Exception(errorCode);
            }
            return value.ToString();
        }

        public void Write(string path, string section, string key, string value)
        {
            WritePrivateProfileString(section,key,value,path);
            var errorCode = Marshal.GetLastWin32Error();
            if(errorCode != 0)
            {
                throw new Win32Exception(errorCode);
            }
        }
    }
}
