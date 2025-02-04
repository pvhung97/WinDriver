using Interop.UIAutomationClient;
using UIADriver.dto.response;
using UIADriver.services;

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
            var wnd = GetCurrentWindow(cacheRequest);
            var hdl = (int)wnd.GetCachedPropertyValue(UIA_PropertyIds.UIA_NativeWindowHandlePropertyId);
            return hdl.ToString();
        }

        public override RectResponse GetCurrentWindowRect()
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_BoundingRectanglePropertyId);
            var wnd = GetCurrentWindow(cacheRequest);
            double[] rect = (double[])wnd.GetCachedPropertyValue(UIA_PropertyIds.UIA_BoundingRectanglePropertyId);
            return new RectResponse((int)rect[0], (int)rect[1], double.IsInfinity(rect[2]) ? 0 : (int)rect[2], double.IsInfinity(rect[3]) ? 0 : (int)rect[3]);
        }

        public override string GetCurrentWindowTitle()
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_NamePropertyId);
            var wnd = GetCurrentWindow(cacheRequest);
            return (string)wnd.GetCachedPropertyValue(UIA_PropertyIds.UIA_NamePropertyId);
        }

        public override string GetCurrentWindowProcessPath()
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_ProcessIdPropertyId);
            var wnd = GetCurrentWindow(cacheRequest);
            var processPath = GetProcessPathFromProcessId((int)wnd.GetCachedPropertyValue(UIA_PropertyIds.UIA_ProcessIdPropertyId));
            if (processPath == null) return "";
            return new Uri(processPath).AbsoluteUri;
        }
    }
}
