using System.Diagnostics;
using System.Windows.Automation;
using UIADriver.dto.response;
using UIADriver.services;
using UIADriver.win32;
using UIADriver.win32native;

namespace UIADriver.uia2.wndmanage
{
    public abstract class UIA2WindowManage : WindowManageService<AutomationElement, CacheRequest>
    {
        public UIA2WindowManage(ServiceProvider<AutomationElement, CacheRequest> serviceProvider) : base(serviceProvider) { }

        public override string GetCurrentWindowHdl()
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(AutomationElement.NativeWindowHandleProperty);
            var wnd = getCurrentWindow(cacheRequest);
            return wnd.Cached.NativeWindowHandle.ToString();
        }

        public override RectResponse GetCurrentWindowRect()
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(AutomationElement.BoundingRectangleProperty);
            var wnd = getCurrentWindow(cacheRequest);
            var rect = wnd.Cached.BoundingRectangle;
            return new RectResponse((int)rect.X, (int)rect.Y, double.IsInfinity(rect.Width) ? 0 : (int)rect.Width, double.IsInfinity(rect.Height) ? 0 : (int)rect.Height);
        }

        public override string GetCurrentWindowTitle()
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(AutomationElement.NameProperty);
            var wnd = getCurrentWindow(cacheRequest);
            return wnd.Cached.Name;
        }
    }
}
