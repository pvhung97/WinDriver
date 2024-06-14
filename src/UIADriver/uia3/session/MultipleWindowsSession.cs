using Interop.UIAutomationClient;
using UIADriver.actions;
using UIADriver.win32;
using UIADriver.win32native;
using UIADriver.uia3.sourcebuilder;
using UIADriver.services;
using UIADriver.uia3.wndmange;
using UIADriver.uia3.actionoptions;

namespace UIADriver.uia3.session
{
    public abstract class MultipleWindowsSession : UIA3Session
    {
        protected MultipleWindowsSession(SessionCapabilities capabilities) : base(capabilities) { }

        protected override PageSourceService<IUIAutomationElement> GetPageSourceService()
        {
            if (PageSourceService == null)
            {
                PageSourceService = new WindowPageSourceBuilder(automation, capabilities, GetElementAttributeService());
            }
            return PageSourceService;
        }
        protected override WindowManageService<IUIAutomationElement, IUIAutomationCacheRequest> GetWindowManageService()
        {
            if (WindowManageService == null)
            {
                WindowManageService = new MultipleWindowManage(automation, GetElementFinderService());
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

        protected override ActionOptions GetActionOption()
        {
            return new UIA3ActionOptions(automation, GetWindowManageService().getCurrentWindow(null), GetElementFinderService());
        }
    }
}
