using Interop.UIAutomationClient;
using UIADriver.actions;
using UIADriver.services;
using UIADriver.uia3.actionoptions;
using UIADriver.uia3.serviceProvider;

namespace UIADriver.uia3.session
{
    public abstract class MultipleWindowsSession : UIA3Session
    {
        protected MultipleWindowsSession(SessionCapabilities capabilities) : base(capabilities) { }

        protected override ServiceProvider<IUIAutomationElement, IUIAutomationCacheRequest> GetServiceProvider()
        {
            if (serviceProvider == null) serviceProvider = new MultipleWindowSessionServiceProvider(capabilities, automation);
            return serviceProvider;
        }

        public override Task CloseSession()
        {
            return Task.Run(() =>
            {
                var wnds = GetServiceProvider().GetWindowManageService().CollectWindowHandles(true);
                foreach (var item in wnds)
                {
                    GetServiceProvider().GetWindowManageService().CloseWindowByHdl(int.Parse(item));
                };
            });
        }

        protected override ActionOptions GetActionOption()
        {
            return new UIA3ActionOptions(automation, GetServiceProvider().GetWindowManageService().GetCurrentWindow(null), GetServiceProvider());
        }
    }
}
