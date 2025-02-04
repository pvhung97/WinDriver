
using Interop.UIAutomationClient;
using UIADriver.actions;
using UIADriver.dto.response;
using UIADriver.services;
using UIADriver.uia3.actionoptions;
using UIADriver.uia3.serviceProvider;

namespace UIADriver.uia3.session
{
    public class RootSession : UIA3Session
    {
        public RootSession(SessionCapabilities capabilities) : base(capabilities) { }

        protected override ServiceProvider<IUIAutomationElement, IUIAutomationCacheRequest> GetServiceProvider()
        {
            if (serviceProvider == null) serviceProvider = new RootSessionServiceProvider(capabilities, automation);
            return serviceProvider;
        }

        public override Task CloseSession()
        {
            return Task.Run(() => { });
        }

        public override HashSet<string> CollectWindowHandles()
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_NativeWindowHandlePropertyId);
            var rootElement = automation.GetRootElementBuildCache(cacheRequest);
            return new HashSet<string>() { ((int)rootElement.GetCachedPropertyValue(UIA_PropertyIds.UIA_NativeWindowHandlePropertyId)).ToString() };
        }

        public override FindElementResponse GetActiveElement()
        {
            return GetServiceProvider().GetElementFinderService().GetActiveElement();
        }

        protected override ActionOptions GetActionOption()
        {
            return new RootSessionActionOptions(automation, GetServiceProvider().GetWindowManageService().GetCurrentWindow(null), GetServiceProvider());
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
