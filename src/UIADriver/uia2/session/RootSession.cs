using System.Windows.Automation;
using UIADriver.actions;
using UIADriver.dto.response;
using UIADriver.services;
using UIADriver.uia2.actionoptions;
using UIADriver.uia2.serviceProvider;

namespace UIADriver.uia2.session
{
    public class RootSession : UIA2Session
    {
        public RootSession(SessionCapabilities capabilities) : base(capabilities) { }

        protected override ServiceProvider<AutomationElement, CacheRequest> GetServiceProvider()
        {
            if (serviceProvider == null) serviceProvider = new RootSessionServiceProvider(capabilities);
            return serviceProvider;
        }

        public override Task CloseSession()
        {
            return Task.Run(() => { });
        }

        public override HashSet<string> CollectWindowHandles()
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(AutomationElement.NativeWindowHandleProperty);
            var rootElement = AutomationElement.RootElement.GetUpdatedCache(cacheRequest);
            return new HashSet<string>() { rootElement.Cached.NativeWindowHandle.ToString() };
        }

        public override FindElementResponse GetActiveElement()
        {
            return GetServiceProvider().GetElementFinderService().GetActiveElement();
        }

        protected override ActionOptions GetActionOption()
        {
            return new RootSessionActionOptions(GetServiceProvider().GetWindowManageService().GetCurrentWindow(null), GetServiceProvider());
        }

        public override string GetScreenshot()
        {
            using (Bitmap bmp = GetServiceProvider().GetScreenCaptureService().CaptureAllMonitor())
            {
                return GetServiceProvider().GetScreenCaptureService().ConvertToBase64(bmp);
            }
        }
    }
}
