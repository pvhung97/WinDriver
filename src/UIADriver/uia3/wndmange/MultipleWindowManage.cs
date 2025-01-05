using Interop.UIAutomationClient;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UIADriver.dto.request;
using UIADriver.dto.response;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.win32;
using UIADriver.win32native;

namespace UIADriver.uia3.wndmange
{
    public class MultipleWindowManage : UIA3WindowManage
    {
        protected IUIAutomationElement? currentWnd;

        public MultipleWindowManage(IUIAutomation automation, ServiceProvider<IUIAutomationElement, IUIAutomationCacheRequest> serviceProvider) : base(automation, serviceProvider) 
        {

        }

        public override List<string> CloseCurrentWindow()
        {
            int currentHdl = (int) getCurrentWindow(null).GetCachedPropertyValue(UIA_PropertyIds.UIA_NativeWindowHandlePropertyId);
            CloseWindowByHdl(currentHdl);
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

        public override IUIAutomationElement getCurrentWindow(IUIAutomationCacheRequest? cacheRequest)
        {
            if (cacheRequest == null)
            {
                cacheRequest = automation.CreateCacheRequest();
                cacheRequest.AddProperty(UIA_PropertyIds.UIA_NativeWindowHandlePropertyId);
                cacheRequest.AddProperty(UIA_PropertyIds.UIA_ProcessIdPropertyId);
            }
            else
            {
                cacheRequest.AddProperty(UIA_PropertyIds.UIA_NativeWindowHandlePropertyId);
                cacheRequest.AddProperty(UIA_PropertyIds.UIA_ProcessIdPropertyId);
            }

            try
            {
                if (currentWnd != null)
                {
                    currentWnd = currentWnd.BuildUpdatedCache(cacheRequest);

                    if ((int)currentWnd.GetCachedPropertyValue(UIA_PropertyIds.UIA_ProcessIdPropertyId) == 0)
                    {
                        throw new NoSuchWindowException("Invalid process id");
                    }
                }
            } catch (Exception ex)
            {
                Debug.WriteLine(ex);
                currentWnd = null;
            }
            if (currentWnd == null) throw new NoSuchWindowException("Current window has already closed or minimized");
            return currentWnd;
        }

        public override IUIAutomationElement getCurrentWindowThenFocus(IUIAutomationCacheRequest? cacheRequest)
        {
            var request = cacheRequest == null ? automation.CreateCacheRequest() : cacheRequest;
            request.AddProperty(UIA_PropertyIds.UIA_NativeWindowHandlePropertyId);
            request.AddProperty(UIA_PropertyIds.UIA_IsWindowPatternAvailablePropertyId);
            var currentWindow = getCurrentWindow(request);

            int hdl = (int)currentWindow.GetCachedPropertyValue(UIA_PropertyIds.UIA_NativeWindowHandlePropertyId);
            nint currentFocus = Win32Methods.GetForegroundWindow();
            if (currentFocus == hdl) return currentWindow;
            //  Only active window that has window pattern
            if ((bool)currentWindow.GetCachedPropertyValue(UIA_PropertyIds.UIA_IsWindowPatternAvailablePropertyId))
            {
                Utilities.BringWindowToTop(hdl);
            }
            return currentWindow;
        }

        public override void InitCurrentWnd(IUIAutomationElement wnd)
        {
            this.currentWnd = wnd;
        }

        public override RectResponse MaximizeCurrentWindow()
        {
            int currentHdl = (int)getCurrentWindow(null).GetCachedPropertyValue(UIA_PropertyIds.UIA_NativeWindowHandlePropertyId);
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
            int currentHdl = (int)getCurrentWindow(null).GetCachedPropertyValue(UIA_PropertyIds.UIA_NativeWindowHandlePropertyId);
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
            int currentHdl = (int)getCurrentWindow(null).GetCachedPropertyValue(UIA_PropertyIds.UIA_NativeWindowHandlePropertyId);
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

        //  Should not active or restore window on switch since context menu will dissapear if other window is activated
        //  To restore window, use maximize or setRect
        public override void SwitchToWindow(SwitchWindowRequest rq)
        {
            int currentHdl = 0;
            try
            {
                currentHdl = (int)getCurrentWindow(null).GetCachedPropertyValue(UIA_PropertyIds.UIA_NativeWindowHandlePropertyId);
            } catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            var wndAndHdl = CollectWindows(true);
            var newHdl = int.Parse(rq.handle);
            var rs = wndAndHdl.Find(wnd => wnd.hdl == newHdl);
            if (rs == null)
            {
                throw new NoSuchWindowException("No such window with handle: " + rq.handle);
            }

            if (newHdl != currentHdl) serviceProvider.GetElementFinderService().ResetCache();
            currentWnd = rs.window;
        }

        public override List<WndHdlAndPid> CollectWindows()
        {
            return CollectWindows(false);
        }

        public override List<WndHdlAndPid> CollectWindows(bool includeIconic)
        {
            List<WndHdlAndPid> rs = [];
            UpdateProcessList();

            var allHwnd = CollectAllTopLevelHdl();

            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_BoundingRectanglePropertyId);
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_NativeWindowHandlePropertyId);
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_ProcessIdPropertyId);

            foreach (var item in allHwnd)
            {
                IUIAutomationElement? fromHdlElement = null;
                try
                {
                    fromHdlElement = automation.ElementFromHandleBuildCache(item, cacheRequest);
                }
                catch { }

                if (fromHdlElement != null)
                {
                    var rect = (double[])fromHdlElement.GetCachedPropertyValue(UIA_PropertyIds.UIA_BoundingRectanglePropertyId);
                    var nativeHdl = (int)fromHdlElement.GetCachedPropertyValue(UIA_PropertyIds.UIA_NativeWindowHandlePropertyId);
                    var pid = (int)fromHdlElement.GetCachedPropertyValue(UIA_PropertyIds.UIA_ProcessIdPropertyId);

                    bool shouldAdd = false;
                    if (pids.Contains(pid))
                    {
                        shouldAdd = true;
                    }
                    else
                    {
                        var ancestorInTree = GetAncesetorWindowInUIATree(fromHdlElement);
                        if (ancestorInTree != null)
                        {
                            ancestorInTree = ancestorInTree.BuildUpdatedCache(cacheRequest);
                            var ancestorPid = (int)ancestorInTree.GetCachedPropertyValue(UIA_PropertyIds.UIA_ProcessIdPropertyId);

                            if (pids.Contains(ancestorPid))
                            {
                                shouldAdd = true;
                            }
                        }
                    }

                    if (shouldAdd)
                    {
                        if (includeIconic)
                        {
                            shouldAdd = true;
                            rs.Add(new WndHdlAndPid(nativeHdl, pid, fromHdlElement));
                        }
                        else if (!Win32Methods.IsIconic(nativeHdl) && !double.IsInfinity(rect[2]) && rect[2] != 0)
                        {
                            shouldAdd = true;
                            rs.Add(new WndHdlAndPid(nativeHdl, pid, fromHdlElement));
                        }
                    }
                }
            }
            return rs;
        }

        public IUIAutomationElement? GetAncesetorWindowInUIATree(IUIAutomationElement current)
        {
            var treeWalker = automation.CreateTreeWalker(automation.CreateTrueCondition());
            var parentPointer = current;
            if (parentPointer == null) return null;

            IUIAutomationElement? ancestorPointer = treeWalker.GetParentElement(parentPointer);
            while (ancestorPointer != null)
            {
                var newAncestor = treeWalker.GetParentElement(ancestorPointer);
                if (newAncestor == null) break;
                parentPointer = ancestorPointer;
                ancestorPointer = newAncestor;
            }
            return parentPointer;
        }

    }
}
