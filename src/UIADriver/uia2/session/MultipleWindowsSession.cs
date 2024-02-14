using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text.Json.Nodes;
using System.Windows.Automation;
using UIADriver.actions;
using UIADriver.dto.request;
using UIADriver.dto.response;
using UIADriver.exception;
using UIADriver.win32;
using UIADriver.win32native;
using UIADriver.uia2.sourcebuilder;

namespace UIADriver.uia2.session
{
    public class MultipleWindowsSession : UIA2Session
    {
        protected int currentHdl = 0;
        protected override PageSourceBuilder PageSourceBuilder => new WindowPageSourceBuilder(attrGetter, capabilities);

        protected MultipleWindowsSession(SessionCapabilities capabilities) : base(capabilities) { }

        public override Task CloseSession()
        {
            List<int> toClose = [];
            UpdateProcessList();
            List<WndHdlAndPid> wnds = CollectWindows(true);

            return Task.Run(() =>
            {
                foreach (var item in wnds)
                {
                    Win32Methods.PostMessage(item.hdl, Win32Constants.WM_CLOSE, nint.Zero, nint.Zero);
                };
            });
        }

        protected List<WndHdlAndPid> CollectWindows()
        {
            return CollectWindows(false);
        }

        private void AssertCurrentWindowExist()
        {
            var wnds = CollectWindows(true);
            var rs = wnds.Find(wnd => wnd.hdl == currentHdl);
            if (rs == null)
            {
                throw new NoSuchWindowException("Current window has already closed");
            }
        }

        protected List<WndHdlAndPid> CollectWindows(bool includeIconic)
        {
            List<WndHdlAndPid> rs = new List<WndHdlAndPid>();
            UpdateProcessList();
            var rootElement = AutomationElement.RootElement;
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(AutomationElement.BoundingRectangleProperty);
            cacheRequest.Add(AutomationElement.NativeWindowHandleProperty);
            cacheRequest.Add(AutomationElement.ProcessIdProperty);
            var walker = new TreeWalker(Condition.TrueCondition);

            var element = walker.GetFirstChild(rootElement, cacheRequest);
            while (element != null)
            {
                try
                {
                    var rect = element.Cached.BoundingRectangle;
                    var nativeHdl = element.Cached.NativeWindowHandle;
                    var pid = element.Cached.ProcessId;

                    if (pids.Contains(pid))
                    {
                        if (includeIconic)
                        {
                            rs.Add(new WndHdlAndPid(nativeHdl, pid, element));
                        }
                        else if (!Win32Methods.IsIconic(nativeHdl) && !double.IsInfinity(rect.Width) && rect.Width != 0)
                        {
                            rs.Add(new WndHdlAndPid(nativeHdl, pid, element));
                        }
                    }
                }
                catch (Exception) { }

                element = walker.GetNextSibling(element, cacheRequest);
            }
            return rs;
        }

        public override HashSet<string> CollectWindowHandles()
        {
            return CollectWindows().Select(i => i.hdl.ToString()).ToHashSet();
        }

        protected override AutomationElement getCurrentWindow(CacheRequest? cacheRequest)
        {
            var windowList = CollectWindows();
            var found = windowList.Find(wnd => wnd.hdl == currentHdl);
            if (found == null) throw new NoSuchWindowException("Current window has already closed or minimized");
            if (cacheRequest != null)
            {
                var updated = found.window.GetUpdatedCache(cacheRequest);
                return updated;
            }
            return found.window;
        }

        protected override AutomationElement getCurrentWindowThenFocus(CacheRequest? cacheRequest)
        {
            var request = cacheRequest == null ? new CacheRequest() : cacheRequest;
            request.Add(AutomationElement.NativeWindowHandleProperty);
            request.Add(AutomationElement.IsWindowPatternAvailableProperty);
            var currentWindow = getCurrentWindow(request);

            int hdl = currentWindow.Cached.NativeWindowHandle;
            nint currentFocus = Win32Methods.GetForegroundWindow();
            if (currentFocus == hdl) return currentWindow;
            //  Only active window that has window pattern
            if ((bool)currentWindow.GetCachedPropertyValue(AutomationElement.IsWindowPatternAvailableProperty))
            {
                Utilities.BringWindowToTop(hdl);
            }
            return currentWindow;
        }

        public override RectResponse MinimizeCurrentWindow()
        {
            AssertCurrentWindowExist();
            if (!Win32Methods.IsIconic(currentHdl))
            {
                Win32Methods.ShowWindow(currentHdl, Win32Constants.SW_SHOWMINIMIZED);
                int lastError = Marshal.GetLastWin32Error();
                if (lastError != 0)
                {
                    string errorMessage = new Win32Exception(lastError).Message;
                    throw new UnknownError(errorMessage);
                }
            }
            return new RectResponse(0, 0, 0, 0);
        }

        public override RectResponse MaximizeCurrentWindow()
        {
            AssertCurrentWindowExist();
            if (Win32Methods.IsIconic(currentHdl)) Win32Methods.ShowWindow(currentHdl, Win32Constants.SW_RESTORE);
            Win32Methods.ShowWindow(currentHdl, Win32Constants.SW_SHOWMAXIMIZED);
            int lastError = Marshal.GetLastWin32Error();
            if (lastError != 0)
            {
                string errorMessage = new Win32Exception(lastError).Message;
                throw new UnknownError(errorMessage);
            }
            return GetCurrentWindowRect();
        }

        public override RectResponse SetWindowRect(JsonObject data)
        {
            var rq = SetRectRequest.Validate(data);
            AssertCurrentWindowExist();
            Win32Methods.ShowWindow(currentHdl, Win32Constants.SW_RESTORE);

            var currentRect = GetCurrentWindowRect();
            if (rq.x == null) rq.x = currentRect.x;
            if (rq.y == null) rq.y = currentRect.y;
            if (rq.width == null) rq.width = currentRect.width;
            if (rq.height == null) rq.height = currentRect.height;

            Win32Methods.MoveWindow(currentHdl, (int)rq.x, (int)rq.y, (int)rq.width, (int)rq.height, true);
            int lastError = Marshal.GetLastWin32Error();
            if (lastError != 0)
            {
                string errorMessage = new Win32Exception(lastError).Message;
                throw new UnknownError(errorMessage);
            }
            return GetCurrentWindowRect();
        }

        public override HashSet<string> CloseCurrentWindow()
        {
            AssertCurrentWindowExist();
            Win32Methods.PostMessage(currentHdl, Win32Constants.WM_CLOSE, nint.Zero, nint.Zero);
            return CollectWindowHandles();
        }

        //  Should not active or restore window on switch since context menu will dissapear if other window is activated
        //  To restore window, use maximize or setRect
        public override void SwitchToWindow(JsonObject data)
        {
            var rq = SwitchWindowRequest.Validate(data);
            var wndAndHdl = CollectWindows(true);
            var newHdl = int.Parse(rq.handle);
            var rs = wndAndHdl.Find(wnd => wnd.hdl == newHdl);
            if (rs == null)
            {
                throw new NoSuchWindowException("No such window with handle: " + rq.handle);
            }

            if (newHdl != currentHdl) elementFinder.resetCache();
            currentHdl = newHdl;
        }

        public override FindElementResponse FindElementFromElement(string elementId, JsonObject data)
        {
            getCurrentWindow(null);
            return base.FindElementFromElement(elementId, data);
        }

        public override List<FindElementResponse> FindElementsFromElement(string elementId, JsonObject data)
        {
            getCurrentWindow(null);
            return base.FindElementsFromElement(elementId, data);
        }

        public override FindElementResponse GetActiveElement()
        {
            getCurrentWindow(null);
            return elementFinder.GetActiveElement(currentHdl);
        }

        public override string? GetElementAttribute(string id, string attribute)
        {
            getCurrentWindow(null);
            return base.GetElementAttribute(id, attribute);
        }

        public override string GetElementTagName(string id)
        {
            getCurrentWindow(null);
            return base.GetElementTagName(id);
        }

        public override RectResponse GetElementRect(string id)
        {
            var currentWindowRect = GetCurrentWindowRect();
            var element = elementFinder.GetElement(id);
            var rect = element.Cached.BoundingRectangle;
            return new RectResponse((int)rect.X - currentWindowRect.x, (int)rect.Y - currentWindowRect.y, double.IsInfinity(rect.Width) ? 0 : (int)rect.Width, double.IsInfinity(rect.Height) ? 0 : (int)rect.Height);
        }

        public override string GetElementText(string id)
        {
            getCurrentWindow(null);
            return base.GetElementText(id);
        }

        public override bool IsElementEnabled(string id)
        {
            getCurrentWindow(null);
            return base.IsElementEnabled(id);
        }

        public override bool IsElementSelected(string id)
        {
            getCurrentWindow(null);
            return base.IsElementSelected(id);
        }

        public override bool IsElementDisplayed(string id)
        {
            getCurrentWindow(null);
            return base.IsElementDisplayed(id);
        }

        protected override ActionOptions getActionOption()
        {
            return new UIA2ActionOptions(getCurrentWindow(null), elementFinder);
        }

        protected class WndHdlAndPid
        {
            public int hdl;
            public int pid;
            public AutomationElement window;

            public WndHdlAndPid(int hdl, int pid, AutomationElement window)
            {
                this.hdl = hdl;
                this.pid = pid;
                this.window = window;
            }
        }
    }
}
