using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Automation;
using UIADriver.dto.request;
using UIADriver.dto.response;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.win32;
using UIADriver.win32native;

namespace UIADriver.uia2.wndmanage
{
    public class MultipleWindowManage : UIA2WindowManage
    {
        protected AutomationElement? currentWnd;

        public MultipleWindowManage(ElementFinderService<AutomationElement, CacheRequest> elementFinder) : base(elementFinder) { }

        public override List<string> CloseCurrentWindow()
        {
            int currentHdl = getCurrentWindow(null).Cached.NativeWindowHandle;
            Win32Methods.PostMessage(currentHdl, Win32Constants.WM_CLOSE, nint.Zero, nint.Zero);
            return CollectWindowHandles();
        }

        public override List<string> CollectWindowHandles()
        {
            return CollectWindowHandles(false);
        }

        public override List<string> CollectWindowHandles(bool includeIconic)
        {
            return CollectWindows(includeIconic).Select(i => i.hdl.ToString()).ToList();
        }

        public override AutomationElement getCurrentWindow(CacheRequest? cacheRequest)
        {
            if (cacheRequest == null)
            {
                cacheRequest = new CacheRequest();
                cacheRequest.Add(AutomationElement.NativeWindowHandleProperty);
            } else cacheRequest.Add(AutomationElement.NativeWindowHandleProperty);

            try
            {
                currentWnd = currentWnd?.GetUpdatedCache(cacheRequest);
            } catch (Exception ex)
            {
                Debug.WriteLine(ex);
                currentWnd = null;
            }
            if (currentWnd == null) throw new NoSuchWindowException("Current window has already closed or minimized");
            return currentWnd;
        }

        public override AutomationElement getCurrentWindowThenFocus(CacheRequest? cacheRequest)
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

        public override void InitCurrentWnd(AutomationElement wnd)
        {
            this.currentWnd = wnd;
        }

        public override RectResponse MaximizeCurrentWindow()
        {
            int currentHdl = getCurrentWindow(null).Cached.NativeWindowHandle;
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

        public override RectResponse MinimizeCurrentWindow()
        {
            int currentHdl = getCurrentWindow(null).Cached.NativeWindowHandle;
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

        public override RectResponse SetWindowRect(SetRectRequest rq)
        {
            int currentHdl = getCurrentWindow(null).Cached.NativeWindowHandle;
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

        public override void SwitchToWindow(SwitchWindowRequest rq)
        {
            int currentHdl = 0;
            try
            {
                currentHdl = getCurrentWindow(null).Cached.NativeWindowHandle;
            } catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            var wndAndHdl = CollectWindows(true);
            var newHdl = int.Parse(rq.handle);
            var rs = wndAndHdl.Find(wnd => wnd.hdl == newHdl);
            if (rs == null)
            {
                throw new NoSuchWindowException("No such window with handle: " + rq.handle);
            }

            if (newHdl != currentHdl) elementFinder.ResetCache();
            currentWnd = rs.window;
        }

        public override List<WndHdlAndPid> CollectWindows()
        {
            return CollectWindows(false);
        }

        public override List<WndHdlAndPid> CollectWindows(bool includeIconic)
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

    }
}
