using System.Windows.Automation;
using UIADriver.services;
using UIADriver.uia2.sourcebuilder;
using UIADriver.uia2.wndmanage;

namespace UIADriver.uia2.serviceProvider
{
    public class RootSessionServiceProvider : UIA2ServiceProvider
    {
        public RootSessionServiceProvider(SessionCapabilities capabilities) : base(capabilities) { }

        public override PageSourceService<AutomationElement, CacheRequest> GetPageSourceService()
        {
            if (PageSourceService == null)
            {
                PageSourceService = new RootPageSourceBuilder(capabilities, this);
            }
            return PageSourceService;
        }

        public override WindowManageService<AutomationElement, CacheRequest> GetWindowManageService()
        {
            if (WindowManageService == null)
            {
                WindowManageService = new RootWindowManage(this);
            }
            return WindowManageService;
        }
    }
}
