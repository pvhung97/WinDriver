using System.Windows.Automation;
using UIADriver.actions;
using UIADriver.win32;
using UIADriver.win32native;
using UIADriver.uia2.sourcebuilder;
using UIADriver.services;
using UIADriver.uia2.wndmanage;
using UIADriver.uia2.actionoptions;

namespace UIADriver.uia2.session
{
    public class MultipleWindowsSession : UIA2Session
    {
        protected MultipleWindowsSession(SessionCapabilities capabilities) : base(capabilities) { }

        protected override PageSourceService<AutomationElement> GetPageSourceService()
        {
            if (PageSourceService == null)
            {
                PageSourceService = new WindowPageSourceBuilder(capabilities, GetElementAttributeService());
            }
            return PageSourceService;
        }
        protected override WindowManageService<AutomationElement, CacheRequest> GetWindowManageService()
        {
            if (WindowManageService == null)
            {
                WindowManageService = new MultipleWindowManage(GetElementFinderService());
            }
            return WindowManageService;
        }

        public override Task CloseSession()
        {
            return Task.Run(() =>
            {
                var wnds = GetWindowManageService().CollectWindowHandles(true);
                foreach (var item in wnds)
                {
                    Win32Methods.PostMessage(int.Parse(item), Win32Constants.WM_CLOSE, nint.Zero, nint.Zero);
                };
            });
        }
        
        protected override ActionOptions getActionOption()
        {
            return new UIA2ActionOptions(GetWindowManageService().getCurrentWindow(null), GetElementFinderService());
        }
    }
}
