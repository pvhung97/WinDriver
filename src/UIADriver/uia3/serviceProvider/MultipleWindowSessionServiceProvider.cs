using Interop.UIAutomationClient;
using UIADriver.services;
using UIADriver.uia3.sourcebuilder;
using UIADriver.uia3.wndmange;

namespace UIADriver.uia3.serviceProvider
{
    public class MultipleWindowSessionServiceProvider : UIA3ServiceProvider
    {
        public MultipleWindowSessionServiceProvider(SessionCapabilities capabilities, IUIAutomation automatiom) : base(capabilities, automatiom) { }

        public override PageSourceService<IUIAutomationElement, IUIAutomationCacheRequest> GetPageSourceService()
        {
            if (PageSourceService == null)
            {
                PageSourceService = new WindowPageSourceBuilder(automation, capabilities, this);
            }
            return PageSourceService;
        }

        public override WindowManageService<IUIAutomationElement, IUIAutomationCacheRequest> GetWindowManageService()
        {
            if (WindowManageService == null)
            {
                WindowManageService = new MultipleWindowManage(automation, this);
            }
            return WindowManageService;
        }
    }
}
