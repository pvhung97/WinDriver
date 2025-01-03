using Interop.UIAutomationClient;
using UIADriver.dto.request;
using UIADriver.dto.response;
using UIADriver.exception;
using UIADriver.services;

namespace UIADriver.uia3.wndmange
{
    public class RootWindowManage : UIA3WindowManage
    {
        private IUIAutomationElement root;

        public RootWindowManage(IUIAutomation automation, ServiceProvider<IUIAutomationElement, IUIAutomationCacheRequest> serviceProvider) : base(automation, serviceProvider)
        {
            this.root = automation.GetRootElement();
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
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_NativeWindowHandlePropertyId);
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_ProcessIdPropertyId);
            var wnd = getCurrentWindow(cacheRequest);
            return [new WndHdlAndPid((int) wnd.GetCachedPropertyValue(UIA_PropertyIds.UIA_NativeWindowHandlePropertyId), (int)wnd.GetCachedPropertyValue(UIA_PropertyIds.UIA_ProcessIdPropertyId), root)];
        }

        public override IUIAutomationElement getCurrentWindow(IUIAutomationCacheRequest? cacheRequest)
        {
            return cacheRequest == null ? root : root.BuildUpdatedCache(cacheRequest);
        }

        public override IUIAutomationElement getCurrentWindowThenFocus(IUIAutomationCacheRequest? cacheRequest)
        {
            return getCurrentWindow(cacheRequest);
        }

        public override void InitCurrentWnd(IUIAutomationElement wnd) { }

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
