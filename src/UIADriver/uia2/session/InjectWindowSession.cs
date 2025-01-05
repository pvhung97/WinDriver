using System.Windows.Automation;
using UIADriver.exception;
using UIADriver.win32native;

namespace UIADriver.uia2.session
{
    public class InjectWindowSession : MultipleWindowsSession
    {
        public InjectWindowSession(SessionCapabilities capabilities) : base(capabilities)
        {
            if (capabilities.nativeWindowHandle == null) throw new SessionNotStartException("Session cannot be created. Cannot find window with handle " + capabilities.aumid);
            if (!GetServiceProvider().GetWindowManageService().IsWindowVisible(capabilities.nativeWindowHandle.Value)) throw new SessionNotStartException("Session cannot be created. Cannot find window with handle " + capabilities.aumid);

            var rootElement = AutomationElement.RootElement;
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(AutomationElement.BoundingRectangleProperty);
            cacheRequest.Add(AutomationElement.NativeWindowHandleProperty);
            cacheRequest.Add(AutomationElement.ProcessIdProperty);

            Tuple<int, int, AutomationElement>? foundWindow = null;
            AutomationElement? element = null;
            try
            {
                element = AutomationElement.FromHandle(capabilities.nativeWindowHandle.Value);
                if (element != null) element = element.GetUpdatedCache(cacheRequest);
            }
            catch { }
            if (element != null)
            {
                var rect = element.Cached.BoundingRectangle;
                var nativeHdl = element.Cached.NativeWindowHandle;
                var pid = element.Cached.ProcessId;

                if (!Win32Methods.IsIconic(nativeHdl) && !double.IsInfinity(rect.Width) && rect.Width != 0 && nativeHdl == capabilities.nativeWindowHandle)
                {
                    foundWindow = Tuple.Create(nativeHdl, pid, element);
                }
            }

            if (foundWindow == null) throw new SessionNotStartException("Session cannot be created. Cannot find any window");
            GetServiceProvider().GetWindowManageService().InitPids(foundWindow.Item2);
            GetServiceProvider().GetWindowManageService().InitCurrentWnd(foundWindow.Item3);
            Utilities.BringWindowToTop(foundWindow.Item1);
            foundWindow.Item3.SetFocus();
        }

        public override Task CloseSession()
        {
            return Task.Run(() => { });
        }
    }
}
