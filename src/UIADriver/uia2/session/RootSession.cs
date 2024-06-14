using System.Windows.Automation;
using UIADriver.actions;
using UIADriver.dto.response;
using UIADriver.services;
using UIADriver.uia2.actionoptions;
using UIADriver.uia2.sourcebuilder;
using UIADriver.uia2.wndmanage;

namespace UIADriver.uia2.session
{
    public class RootSession : UIA2Session
    {
        public RootSession(SessionCapabilities capabilities) : base(capabilities) { }

        protected override PageSourceService<AutomationElement> GetPageSourceService()
        {
            if (PageSourceService == null)
            {
                PageSourceService = new RootPageSourceBuilder(capabilities, GetElementAttributeService());
            }
            return PageSourceService;
        }
        protected override WindowManageService<AutomationElement, CacheRequest> GetWindowManageService()
        {
            if (WindowManageService == null)
            {
                WindowManageService = new RootWindowManage(GetElementFinderService());
            }
            return WindowManageService;
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
            return GetElementFinderService().GetActiveElement();
        }

        protected override ActionOptions GetActionOption()
        {
            return new RootSessionActionOptions(GetWindowManageService().getCurrentWindow(null), GetElementFinderService());
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
