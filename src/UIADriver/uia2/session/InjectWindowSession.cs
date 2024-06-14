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

            Tuple<int, int, AutomationElement>? foundWindow = null;
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
                        foundWindow = Tuple.Create(nativeHdl, pid, element);
                        break;
                    }
                }

                element = walker.GetNextSibling(element, cacheRequest);
            }

            if (foundWindow == null) throw new SessionNotStartException("Session cannot be created. Cannot find any window");
            GetWindowManageService().InitPids(foundWindow.Item2);
            GetWindowManageService().InitCurrentWnd(foundWindow.Item3);
            Utilities.BringWindowToTop(foundWindow.Item1);
            foundWindow.Item3.SetFocus();
        }

        public override Task CloseSession()
        {
            return Task.Run(() => { });
        }
    }
}
