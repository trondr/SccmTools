using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace SccmTools.Library.Common.IO
{
    public class PathOperation : IPathOperation
    {
        public string GetUncPath(string path)
        {
            string drive = null;
            if (path[1] == ':')
            {
                drive = path.Substring(0, 2);
            }
            if (!string.IsNullOrEmpty(drive))
            {
                path = path.TrimEnd(Path.DirectorySeparatorChar);
                var driveInfo = new DriveInfo(drive);
                if (driveInfo.DriveType == DriveType.Network)
                {
                    //A network drive get unc path               
                    string uncPath;
                    var buffer = IntPtr.Zero;
                    try
                    {
                        // Call WNetGetUniversalName to get buffer size.
                        var size = 0;
                        var apiReturnValue = WNetGetUniversalName(path, UniversalNameInfoLevel, (IntPtr) IntPtr.Size,
                            ref size);
                        if (apiReturnValue != ErrorMoreData)
                        {
                            throw new Win32Exception(apiReturnValue);
                        }
                        // Allocate the buffer.
                        buffer = Marshal.AllocCoTaskMem(size);

                        // Now make the call.
                        apiReturnValue = WNetGetUniversalName(path, UniversalNameInfoLevel, buffer, ref size);

                        if (apiReturnValue != NoError)
                        {
                            throw new Win32Exception(apiReturnValue);
                        }

                        // Get the string.  
                        uncPath = Marshal.PtrToStringAnsi(new IntPtr(buffer.ToInt64() + IntPtr.Size), size);
                        uncPath = uncPath.Substring(0, uncPath.IndexOf('\0'));
                    }
                    finally
                    {
                        // Release buffer.
                        Marshal.FreeCoTaskMem(buffer);
                    }
                    return uncPath;
                }
                if (path.Length > 2)
                {
                    return string.Format(@"\\{0}\{1}${2}", Environment.GetEnvironmentVariable("COMPUTERNAME"), path[0], path.Substring(2, path.Length - 2));
                }
                return string.Format(@"\\{0}\{1}$", Environment.GetEnvironmentVariable("COMPUTERNAME"), path[0]);
            }
            return path;
        }

        private const int UniversalNameInfoLevel = 0x00000001;
        private const int ErrorMoreData = 234;
        private const int NoError = 0;

        [DllImport("mpr.dll")]
        [return: MarshalAs(UnmanagedType.U4)]
        public static extern int WNetGetUniversalName(string lpLocalPath, [MarshalAs(UnmanagedType.U4)] int dwInfoLevel, IntPtr lpBuffer, [MarshalAs(UnmanagedType.U4)] ref int lpBufferSize);
    }
}