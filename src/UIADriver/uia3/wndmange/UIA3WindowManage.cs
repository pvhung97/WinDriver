using Interop.UIAutomationClient;
using System.Diagnostics;
using UIADriver.dto.response;
using UIADriver.services;
using UIADriver.win32;
using UIADriver.win32native;

namespace UIADriver.uia3.wndmange
{
    public abstract class UIA3WindowManage : WindowManageService<IUIAutomationElement, IUIAutomationCacheRequest>
    {
        protected IUIAutomation automation;

        public UIA3WindowManage(IUIAutomation automation, ServiceProvider<IUIAutomationElement, IUIAutomationCacheRequest> serviceProvider) : base(serviceProvider)
        { 
            this.automation = automation;
        }

        public override string GetCurrentWindowHdl()
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_NativeWindowHandlePropertyId);
            var wnd = getCurrentWindow(cacheRequest);
            var hdl = (int)wnd.GetCachedPropertyValue(UIA_PropertyIds.UIA_NativeWindowHandlePropertyId);
            return hdl.ToString();
        }

        public override RectResponse GetCurrentWindowRect()
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_BoundingRectanglePropertyId);
            var wnd = getCurrentWindow(cacheRequest);
            double[] rect = (double[])wnd.GetCachedPropertyValue(UIA_PropertyIds.UIA_BoundingRectanglePropertyId);
            return new RectResponse((int)rect[0], (int)rect[1], double.IsInfinity(rect[2]) ? 0 : (int)rect[2], double.IsInfinity(rect[3]) ? 0 : (int)rect[3]);
        }

        public override string GetCurrentWindowTitle()
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_NamePropertyId);
            var wnd = getCurrentWindow(cacheRequest);
            return (string)wnd.GetCachedPropertyValue(UIA_PropertyIds.UIA_NamePropertyId);
        }
    }
}
