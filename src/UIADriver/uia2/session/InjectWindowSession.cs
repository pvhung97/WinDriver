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

            var rootElement = AutomationElement.RootElement;
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(AutomationElement.BoundingRectangleProperty);
            cacheRequest.Add(AutomationElement.NativeWindowHandleProperty);
            cacheRequest.Add(AutomationElement.ProcessIdProperty);
            var walker = new TreeWalker(Condition.TrueCondition);

            WndHdlAndPid? foundWindow = null;
            var element = walker.GetFirstChild(rootElement, cacheRequest);
            while (element != null)
            {
                var rect = element.Cached.BoundingRectangle;
                var nativeHdl = element.Cached.NativeWindowHandle;
                var pid = element.Cached.ProcessId;

                if (!Win32Methods.IsIconic(nativeHdl) && !double.IsInfinity(rect.Width) && rect.Width != 0)
                {
                    if (nativeHdl == capabilities.nativeWindowHandle)
                    {
                        foundWindow = new WndHdlAndPid(nativeHdl, pid, element);
                        break;
                    }
                }

                element = walker.GetNextSibling(element, cacheRequest);
            }

            if (foundWindow == null) throw new SessionNotStartException("Session cannot be created. Cannot find any window");
            pids.Add(foundWindow.pid);
            currentHdl = foundWindow.hdl;
            Utilities.BringWindowToTop(currentHdl);
        }

        public override Task CloseSession()
        {
            return Task.Run(() => { });
        }
    }
}
