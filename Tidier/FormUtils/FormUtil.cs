using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Tidier.FormUtils
{
    public class FormUtil
    {
        public static string AppAlreadyRunningMessage
        {
            get { return Application.ProductName + " is already running on this machine. "; }
        }

        public static string AppNameWithVersion
        {
            get { return Application.ProductName + " " + (new Version(Application.ProductVersion)).ToString(2); }
        }

        /// <summary>
        /// Determines whether application is already running.
        /// </summary>
        public static bool IsAppAlreadyRunning()
        {
            var current = Process.GetCurrentProcess();
            var processes = Process.GetProcessesByName(current.ProcessName);

            //check if already running (concurrency issues!)
            if (processes.Length > 1)
                return true;
            return false;
        }
    }
}
