
using Interop.UIAutomationClient;
using UIADriver.actions;
using UIADriver.dto.response;
using UIADriver.services;
using UIADriver.uia3.actionoptions;
using UIADriver.uia3.sourcebuilder;
using UIADriver.uia3.wndmange;

namespace UIADriver.uia3.session
{
    public class RootSession : UIA3Session
    {
        public RootSession(SessionCapabilities capabilities) : base(capabilities) { }

        protected override PageSourceService<IUIAutomationElement> GetPageSourceService()
        {
            if (PageSourceService == null)
            {
                PageSourceService = new RootPageSourceBuilder(automation, capabilities, GetElementAttributeService());
            }
            return PageSourceService;
        }
        protected override WindowManageService<IUIAutomationElement, IUIAutomationCacheRequest> GetWindowManageService()
        {
            if (WindowManageService == null)
            {
                WindowManageService = new RootWindowManage(automation, GetElementFinderService());
            }
            return WindowManageService;
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
            return GetElementFinderService().GetActiveElement();
        }

        protected override ActionOptions GetActionOption()
        {
            return new RootSessionActionOptions(automation, GetWindowManageService().getCurrentWindow(null), GetElementFinderService());
        }

        public override string GetScreenshot()
        {
            using (Bitmap bmp = GetScreenCaptureService().CaptureAllMonitor())
            {
                return GetScreenCaptureService().ConvertToBase64(bmp);
            }
        }
    }
}
