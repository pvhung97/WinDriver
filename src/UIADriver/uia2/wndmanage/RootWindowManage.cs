using System.Windows.Automation;
using UIADriver.dto.request;
using UIADriver.dto.response;
using UIADriver.exception;
using UIADriver.services;

namespace UIADriver.uia2.wndmanage
{
    public class RootWindowManage : UIA2WindowManage
    {
        protected AutomationElement root;

        public RootWindowManage(ServiceProvider<AutomationElement, CacheRequest> serviceProvider) : base(serviceProvider)
        {
            root = AutomationElement.RootElement;
        }

        public override List<string> CloseCurrentWindow()
        {
            throw new UnsupportedOperation("Cannot close window in root session");
        }

        public override List<string> CollectWindowHandles()
        {
            return CollectWindowHandles(false);
        }

        public override List<string> CollectWindowHandles(bool includeIconic)
        {
            return new List<string>() { GetCurrentWindowHdl() };
        }

        public override List<WndHdlAndPid> CollectWindows()
        {
            return CollectWindows(false);
        }

        public override List<WndHdlAndPid> CollectWindows(bool includeIconic)
        {
            CacheRequest cacheRequest = new CacheRequest();
            cacheRequest.Add(AutomationElement.NativeWindowHandleProperty);
            cacheRequest.Add(AutomationElement.ProcessIdProperty);
            var wnd = getCurrentWindow(cacheRequest);
            return [new WndHdlAndPid(wnd.Cached.NativeWindowHandle, wnd.Cached.ProcessId, root)];
        }

        public override AutomationElement getCurrentWindow(CacheRequest? cacheRequest)
        {
            return cacheRequest == null ? AutomationElement.RootElement : AutomationElement.RootElement.GetUpdatedCache(cacheRequest);
        }

        public override AutomationElement getCurrentWindowThenFocus(CacheRequest? cacheRequest)
        {
            return getCurrentWindow(cacheRequest);
        }

        public override void InitCurrentWnd(AutomationElement wnd) { }

        public override RectResponse MaximizeCurrentWindow()
        {
            throw new UnsupportedOperation("Cannot maximize in root session");
        }

        public override RectResponse MinimizeCurrentWindow()
        {
            throw new UnsupportedOperation("Cannot minimize in root session");
        }

        public override RectResponse SetWindowRect(SetRectRequest data)
        {
            throw new UnsupportedOperation("Cannot set window rect in root session");
        }

        public override void SwitchToWindow(SwitchWindowRequest windowHandle)
        {
            throw new UnsupportedOperation("Cannot switch window on root session");
        }
    }
}
