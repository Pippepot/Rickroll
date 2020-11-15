using System;
using System.Runtime.InteropServices;

namespace RickRoll
{
    internal static class ProcessLauncher
    {
        internal static void StartProcessAsUser(string executablePath, string commandline, IntPtr sessionTokenHandle)
        {
            var processInformation = new NativeMethods.PROCESS_INFORMATION();
            try
            {
                var startupInformation = new NativeMethods.STARTUPINFO();
                startupInformation.length = Marshal.SizeOf(startupInformation);
                startupInformation.desktop = string.Empty;
                bool result = NativeMethods.CreateProcessAsUser
                (
                    sessionTokenHandle,
                    executablePath,
                    commandline,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    false,
                    0,
                    IntPtr.Zero,
                    null,
                    ref startupInformation,
                    ref processInformation
                );
                if (!result)
                {
                    int error = Marshal.GetLastWin32Error();
                    string message = string.Format("CreateProcessAsUser Error: {0}", error);
                    throw new ApplicationException(message);
                }
            }
            finally
            {
                if (processInformation.processHandle != IntPtr.Zero)
                {
                    NativeMethods.CloseHandle(processInformation.processHandle);
                }
                if (processInformation.threadHandle != IntPtr.Zero)
                {
                    NativeMethods.CloseHandle(processInformation.threadHandle);
                }
                if (sessionTokenHandle != IntPtr.Zero)
                {
                    NativeMethods.CloseHandle(sessionTokenHandle);
                }
            }
        }
    }

    internal static class NativeMethods
    {
        [DllImport("kernel32.dll", EntryPoint = "CloseHandle", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool CloseHandle(IntPtr handle);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool CreateProcessAsUser(IntPtr tokenHandle, string applicationName, string commandLine, IntPtr processAttributes, IntPtr threadAttributes, bool inheritHandle, int creationFlags, IntPtr envrionment, string currentDirectory, ref STARTUPINFO startupInfo, ref PROCESS_INFORMATION processInformation);

        [DllImport("Kernel32.dll", EntryPoint = "WTSGetActiveConsoleSessionId")]
        internal static extern int WTSGetActiveConsoleSessionId();

        [DllImport("WtsApi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool WTSQueryUserToken(int SessionId, out IntPtr phToken);

        [StructLayout(LayoutKind.Sequential)]
        internal struct PROCESS_INFORMATION
        {
            public IntPtr processHandle;
            public IntPtr threadHandle;
            public int processID;
            public int threadID;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct STARTUPINFO
        {
            public int length;
            public string reserved;
            public string desktop;
            public string title;
            public int x;
            public int y;
            public int width;
            public int height;
            public int consoleColumns;
            public int consoleRows;
            public int consoleFillAttribute;
            public int flags;
            public short showWindow;
            public short reserverd2;
            public IntPtr reserved3;
            public IntPtr stdInputHandle;
            public IntPtr stdOutputHandle;
            public IntPtr stdErrorHandle;
        }
    }

}