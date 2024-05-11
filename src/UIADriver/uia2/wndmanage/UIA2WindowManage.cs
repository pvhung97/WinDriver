using System.Windows.Automation;
using UIADriver.dto.response;
using UIADriver.services;

namespace UIADriver.uia2.wndmanage
{
    public abstract class UIA2WindowManage : WindowManageService<AutomationElement, CacheRequest>
    {
        public UIA2WindowManage(ElementFinderService<AutomationElement, CacheRequest> elementFinder) : base(elementFinder) { }

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
