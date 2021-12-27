using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace RickRoll
{
    public partial class RicksService : ServiceBase
    {
        const int msInterval = 30000; // 30 seconds

        public RicksService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Timer timer = new Timer(msInterval);
            timer.Elapsed += new ElapsedEventHandler(Execute);
            timer.Start();
        }
    

        public void Execute(object source, ElapsedEventArgs e)
        {
            IntPtr sessionTokenHandle = IntPtr.Zero;
            try
            {
                sessionTokenHandle = SessionFinder.GetLocalInteractiveSession();
                if (sessionTokenHandle != IntPtr.Zero)
                {
                    ProcessLauncher.StartProcessAsUser("cmd.exe", "cmd.exe /C start iexplore https://www.youtube.com/watch?v=oHg5SJYRHA0&t=1s", sessionTokenHandle);
                }
            }
            finally
            {
                if (sessionTokenHandle != IntPtr.Zero)
                {
                    NativeMethods.CloseHandle(sessionTokenHandle);
                }
            }
        }

        internal static class SessionFinder
        {
            private const int INT_ConsoleSession = -1;

            internal static IntPtr GetLocalInteractiveSession()
            {
                IntPtr tokenHandle = IntPtr.Zero;
                int sessionID = NativeMethods.WTSGetActiveConsoleSessionId();
                if (sessionID != INT_ConsoleSession)
                {
                    if (!NativeMethods.WTSQueryUserToken(sessionID, out tokenHandle))
                    {
                        throw new System.ComponentModel.Win32Exception();
                    }
                }
                return tokenHandle;
            }
        }
    }
}
