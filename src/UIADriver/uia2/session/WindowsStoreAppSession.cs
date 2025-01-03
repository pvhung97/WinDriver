using System.Text;
using System.Windows.Automation;
using UIADriver.exception;
using UIADriver.win32native;

namespace UIADriver.uia2.session
{
    public class WindowsStoreAppSession : MultipleWindowsSession
    {
        public WindowsStoreAppSession(SessionCapabilities capabilities) : base(capabilities)
        {
            if (capabilities.aumid == null || capabilities.aumid.Length == 0) throw new SessionNotStartException("Session cannot be created. Cannot launch app with aumid " + capabilities.aumid);
            var launcher = new ApplicationActivationManager();
            var argumentBuilder = new StringBuilder();
            foreach (var argument in capabilities.appArgument)
            {
                argumentBuilder.Append("\"");
                string escaped = argument.Replace("\\", "\\\\").Replace("\"", "\\\"");
                argumentBuilder.Append(escaped);
                argumentBuilder.Append("\" ");
            }
            launcher.ActivateApplication(capabilities.aumid, argumentBuilder.ToString(), ActivateOptions.None, out var pidW32);
            if (pidW32 == 0) throw new SessionNotStartException("Session cannot be created. Cannot launch app with aumid " + capabilities.aumid);
            Thread.Sleep(capabilities.delayAfterOpenApp);
            int pid = Convert.ToInt32(pidW32);
            var pids = GetServiceProvider().GetWindowManageService().InitPids(pid);
            var foundWindow = SearchForWindowLaunchedByApp(pids);
            if (foundWindow == null) throw new SessionNotStartException("Session cannot be created. Cannot find any window");
            GetServiceProvider().GetWindowManageService().InitPids(foundWindow.Item2);
            GetServiceProvider().GetWindowManageService().InitCurrentWnd(foundWindow.Item3);
            Utilities.BringWindowToTop(foundWindow.Item1);
            foundWindow.Item3.SetFocus();
        }

        private Tuple<int, int, AutomationElement>? SearchForWindowLaunchedByApp(HashSet<int> pids)
        {
            var rootElement = AutomationElement.RootElement;
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(AutomationElement.BoundingRectangleProperty);
            cacheRequest.Add(AutomationElement.NativeWindowHandleProperty);
            cacheRequest.Add(AutomationElement.ProcessIdProperty);
            var walker = new TreeWalker(Condition.TrueCondition);

            var element = walker.GetFirstChild(rootElement, cacheRequest);
            while (element != null)
            {
                var rect = element.Cached.BoundingRectangle;
                var nativeHdl = element.Cached.NativeWindowHandle;
                var pid = element.Cached.ProcessId;

                if (!Win32Methods.IsIconic(nativeHdl) && !double.IsInfinity(rect.Width) && rect.Width != 0)
                {
                    if (pids.Contains(pid)) return Tuple.Create(nativeHdl, pid, element);

                    var childPointer = walker.GetFirstChild(element, cacheRequest);
                    while (childPointer != null)
                    {
                        var pidChild = childPointer.Cached.ProcessId;

                        if (pids.Contains(pidChild)) return Tuple.Create(nativeHdl, pid, element);

                        childPointer = walker.GetNextSibling(childPointer, cacheRequest);
                    }

                }


                element = walker.GetNextSibling(element, cacheRequest);
            }
            return null;
        }
    }
}
