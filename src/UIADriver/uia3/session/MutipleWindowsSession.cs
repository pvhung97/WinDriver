using Interop.UIAutomationClient;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text.Json.Nodes;
using UIA3Driver;
using UIA3Driver.actions;
using UIA3Driver.dto.request;
using UIA3Driver.dto.response;
using UIA3Driver.exception;
using UIA3Driver.win32;
using UIA3Driver.win32native;
using UIADriver.uia3.sourcebuilder;

namespace UIADriver.uia3.session
{
    public abstract class MutipleWindowsSession : UIA3Session
    {
        protected int currentHdl = 0;
        protected override PageSourceBuilder PageSourceBuilder => new WindowPageSourceBuilder(automation, attrGetter, capabilities);

        protected MutipleWindowsSession(SessionCapabilities capabilities) : base(capabilities) { }

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
            var rootElement = automation.GetRootElement();
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_BoundingRectanglePropertyId);
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_NativeWindowHandlePropertyId);
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_ProcessIdPropertyId);
            var walker = automation.CreateTreeWalker(automation.CreateTrueCondition());

            var element = walker.GetFirstChildElementBuildCache(rootElement, cacheRequest);
            while (element != null)
            {
                try
                {
                    var rect = (double[])element.GetCachedPropertyValue(UIA_PropertyIds.UIA_BoundingRectanglePropertyId);
                    var nativeHdl = (int)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_NativeWindowHandlePropertyId);
                    var pid = (int)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_ProcessIdPropertyId);

                    if (pids.Contains(pid))
                    {
                        if (includeIconic)
                        {
                            rs.Add(new WndHdlAndPid(nativeHdl, pid, element));
                        } else if (!Win32Methods.IsIconic(nativeHdl) && !double.IsInfinity(rect[2]) && rect[2] != 0)
                        {
                            rs.Add(new WndHdlAndPid(nativeHdl, pid, element));
                        }
                    }
                }
                catch (Exception) { }

                element = walker.GetNextSiblingElementBuildCache(element, cacheRequest);
            }
            return rs;
        }

        public override HashSet<string> CollectWindowHandles()
        {
            return CollectWindows().Select(i => i.hdl.ToString()).ToHashSet();
        }

        protected override IUIAutomationElement getCurrentWindow(IUIAutomationCacheRequest? cacheRequest)
        {
            var windowList = CollectWindows();
            var found = windowList.Find(wnd => wnd.hdl == currentHdl);
            if (found == null) throw new NoSuchWindowException("Current window has already closed or minimized");
            if (cacheRequest != null)
            {
                var updated = found.window.BuildUpdatedCache(cacheRequest);
                return updated;
            }
            return found.window;
        }

        protected override IUIAutomationElement getCurrentWindowThenFocus(IUIAutomationCacheRequest? cacheRequest)
        {
            var request = cacheRequest == null ? automation.CreateCacheRequest() : cacheRequest;
            request.AddProperty(UIA_PropertyIds.UIA_NativeWindowHandlePropertyId);
            var currentWindow = getCurrentWindow(request);

            int hdl = (int)currentWindow.GetCachedPropertyValue(UIA_PropertyIds.UIA_NativeWindowHandlePropertyId);
            nint currentFocus = Win32Methods.GetForegroundWindow();
            if (currentFocus == hdl) return currentWindow;
            Win32Methods.SetForegroundWindow(hdl);
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
            if (Win32Methods.IsIconic(currentHdl)) Win32Methods.ShowWindow(currentHdl, Win32Constants.SW_RESTORE);
            Win32Methods.SetForegroundWindow(currentHdl);
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
            double[] rect = (double[])element.GetCachedPropertyValue(UIA_PropertyIds.UIA_BoundingRectanglePropertyId);
            return new RectResponse((int)rect[0] - currentWindowRect.x, (int)rect[1] - currentWindowRect.y, double.IsInfinity(rect[2]) ? 0 : (int)rect[2], double.IsInfinity(rect[3]) ? 0 : (int)rect[3]);
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
            return new UIA3ActionOptions(automation, getCurrentWindow(null), elementFinder);
        }

        protected class WndHdlAndPid
        {
            public int hdl;
            public int pid;
            public IUIAutomationElement window;

            public WndHdlAndPid(int hdl, int pid, IUIAutomationElement window)
            {
                this.hdl = hdl;
                this.pid = pid;
                this.window = window;
            }
        }
    }
}
