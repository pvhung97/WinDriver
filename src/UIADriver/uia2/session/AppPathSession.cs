﻿using System.Diagnostics;
using System.IO;
using UIADriver.exception;

namespace UIADriver.uia2.session
{
    public class AppPathSession : MultipleWindowsSession
    {
        public AppPathSession(SessionCapabilities capabilities) : base(capabilities)
        {
            if (!File.Exists(capabilities.appPath)) throw new SessionNotStartException(capabilities.appPath + " does not exists");
            var p = new Process();
            p.StartInfo.WorkingDirectory = capabilities.workingDirectory != null ? capabilities.workingDirectory : Path.GetDirectoryName(capabilities.appPath);
            p.StartInfo.FileName = capabilities.appPath;
            p.StartInfo.UseShellExecute = true;
            foreach (var arg in capabilities.appArgument)
            {
                p.StartInfo.ArgumentList.Add(arg);
            }
            p.Start();

            Thread.Sleep(capabilities.delayAfterOpenApp);
            var pids = GetServiceProvider().GetWindowManageService().InitPids(p.Id);
            if (pids.Count == 1 && p.HasExited) throw new SessionNotStartException("Session cannot be created. Check if provided path can be execute");
            var windows = GetServiceProvider().GetWindowManageService().CollectWindows();
            if (windows.Count == 0) throw new SessionNotStartException("Session cannot be created. Cannot find any window");
            GetServiceProvider().GetWindowManageService().InitCurrentWnd(windows[0].window);
            Utilities.BringWindowToTop(windows[0].hdl);
            windows[0].window.SetFocus();
        }
    }
}
