﻿using System.Diagnostics;
using UIA3Driver;
using UIA3Driver.exception;
using UIA3Driver.win32native;

namespace UIADriver.uia3.session
{
    public class AppPathSession : MutipleWindowsSession
    {
        public AppPathSession(SessionCapabilities capabilities) : base(capabilities)
        {
            if (!File.Exists(capabilities.appPath)) throw new SessionNotStartException(capabilities.appPath + " does not exists");
            var p = new Process();
            p.StartInfo.WorkingDirectory = capabilities.workingDirectory != null ? capabilities.workingDirectory : Path.GetDirectoryName(capabilities.appPath);
            p.StartInfo.FileName = capabilities.appPath;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            foreach (var arg in capabilities.appArgument)
            {
                p.StartInfo.ArgumentList.Add(arg);
            }
            p.Start();

            Thread.Sleep(capabilities.delayAfterOpenApp);
            pids.Add(p.Id);
            UpdateProcessList();
            if (pids.Count == 1 && p.HasExited) throw new SessionNotStartException("Session cannot be created. Check if provided path can be execute");
            List<WndHdlAndPid> windows = CollectWindows();
            if (windows.Count == 0) throw new SessionNotStartException("Session cannot be created. Cannot find any window");
            currentHdl = windows[0].hdl;
            Win32Methods.SetForegroundWindow(currentHdl);
        }

    }
}
