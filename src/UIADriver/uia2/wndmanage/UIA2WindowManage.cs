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
            var wnd = GetCurrentWindow(cacheRequest);
            return wnd.Cached.NativeWindowHandle.ToString();
        }

        public override RectResponse GetCurrentWindowRect()
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(AutomationElement.BoundingRectangleProperty);
            var wnd = GetCurrentWindow(cacheRequest);
            var rect = wnd.Cached.BoundingRectangle;
            return new RectResponse((int)rect.X, (int)rect.Y, double.IsInfinity(rect.Width) ? 0 : (int)rect.Width, double.IsInfinity(rect.Height) ? 0 : (int)rect.Height);
        }

        public override string GetCurrentWindowTitle()
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(AutomationElement.NameProperty);
            var wnd = GetCurrentWindow(cacheRequest);
            return wnd.Cached.Name;
        }

        public override string GetCurrentWindowProcessPath()
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(AutomationElement.ProcessIdProperty);
            var wnd = GetCurrentWindow(cacheRequest);
            var processPath = GetProcessPathFromProcessId(wnd.Cached.ProcessId);
            if (processPath == null) return "";
            return new Uri(processPath).AbsoluteUri;
        }
    }
}
