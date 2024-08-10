using System.Text.RegularExpressions;
using System.Windows.Automation;
using UIADriver.exception;
using UIADriver.win32native;

namespace UIADriver.uia2.session
{
    public class InjectByWindowNameSession : MultipleWindowsSession
    {
        public InjectByWindowNameSession(SessionCapabilities capabilities) : base(capabilities)
        {
            if (capabilities.windowNameRegex == null) throw new SessionNotStartException("Session cannot be created. Cannot find window with name match regex " + capabilities.windowNameRegex);

            var rootElement = AutomationElement.RootElement;
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(AutomationElement.BoundingRectangleProperty);
            cacheRequest.Add(AutomationElement.NameProperty);
            cacheRequest.Add(AutomationElement.NativeWindowHandleProperty);
            cacheRequest.Add(AutomationElement.ProcessIdProperty);
            var walker = new TreeWalker(Condition.TrueCondition);

            List<Tuple<int, int, AutomationElement>> foundWindows = [];
            var element = walker.GetFirstChild(rootElement, cacheRequest);
            while (element != null)
            {
                var rect = element.Cached.BoundingRectangle;
                var nativeHdl = element.Cached.NativeWindowHandle;
                var pid = element.Cached.ProcessId;
                var windowName = element.Cached.Name;

                if (!Win32Methods.IsIconic(nativeHdl) && !double.IsInfinity(rect.Width) && rect.Width != 0)
                {
                    var match = Regex.Match(windowName, capabilities.windowNameRegex);
                    if (match.Success && match.Value.Length == windowName.Length)
                    {
                        foundWindows.Add(Tuple.Create(nativeHdl, pid, element));
                    }
                }

                element = walker.GetNextSibling(element, cacheRequest);
            }

            if (foundWindows.Count == 0) throw new SessionNotStartException("Session cannot be created. Cannot find any window");
            GetWindowManageService().InitPids(foundWindows.Select(item => item.Item2).ToList());
            GetWindowManageService().InitCurrentWnd(foundWindows[0].Item3);
            Utilities.BringWindowToTop(foundWindows[0].Item1);
            foundWindows[0].Item3.SetFocus();
        }

        public override Task CloseSession()
        {
            return Task.Run(() => { });
        }
    }
}
