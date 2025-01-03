using System.Windows.Automation;
using UIADriver.actions;
using UIADriver.services;
using UIADriver.uia2.actionoptions;
using UIADriver.uia2.serviceProvider;

namespace UIADriver.uia2.session
{
    public class MultipleWindowsSession : UIA2Session
    {
        protected MultipleWindowsSession(SessionCapabilities capabilities) : base(capabilities) { }

        protected override ServiceProvider<AutomationElement, CacheRequest> GetServiceProvider()
        {
            if (serviceProvider == null) serviceProvider = new MultipleWindowSessionServiceProvider(capabilities);
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
            return new UIA2ActionOptions(GetServiceProvider().GetWindowManageService().getCurrentWindow(null), GetServiceProvider());
        }
    }
}
