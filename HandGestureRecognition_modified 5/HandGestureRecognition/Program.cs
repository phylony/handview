using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;


namespace HandGestureRecognition
{
    static class Program
    {
        //boolean to indicate application has initial ownership of mutex
        private static bool isNew;
        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Deny, UnmanagedCode = true)]
        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
        public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, uint pvParam, uint fWinIni);
        [DllImport("User32.dll", EntryPoint = "ShowWindowAsync")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);
        
        private const int WS_SHOWNORMAL = 1;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
                        //Create mutex at application start and try to get the ownership
            using (var m = new System.Threading.Mutex(true, "HandGestureRecognition", out isNew))
            {
                //If application owns the mutex, continue the execution
                if (isNew)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new Form1());
                }
                //else show user message that application is running and set focus to that application window
                else
                {
                    MessageBox.Show("Application already running", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Process current = Process.GetCurrentProcess();

                    foreach (Process process in Process.GetProcessesByName(current.ProcessName))
                    {
                        
                        if (process.Id != current.Id)
                        {

                            SystemParametersInfo((uint)0x2001, 0, 0, 0x0002 | 0x0001);
                            ShowWindowAsync(process.MainWindowHandle, WS_SHOWNORMAL);
                            SetForegroundWindow(process.MainWindowHandle);
                            SystemParametersInfo((uint)0x2001, 200000, 200000, 0x0002 | 0x0001);
                            break;
                        }
                    }
                }
            }
        }
    }
}
