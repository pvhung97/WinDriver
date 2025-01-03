using Interop.UIAutomationClient;
using UIADriver.dto.response;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia3.pattern
{
    public class UIA3BasePattern : BasePatternService<IUIAutomationElement, IUIAutomationCacheRequest>
    {
        protected IUIAutomation automation;
        public UIA3BasePattern(ServiceProvider<IUIAutomationElement, IUIAutomationCacheRequest> serviceProvider, IUIAutomation automation) : base(serviceProvider)
        {
            this.automation = automation;
        }

        public override List<FindElementResponse> GetControllerFor(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_ControllerForPropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var elementArr = element.CachedControllerFor;
            List<FindElementResponse> rs = [];
            if (elementArr != null)
            {
                for (int i = 0; i < elementArr.Length; i++)
                {
                    try
                    {
                        string itemId = serviceProvider.GetElementFinderService().RegisterElement(elementArr.GetElement(i));
                        rs.Add(new FindElementResponse(itemId));
                    }
                    catch { }
                }
            }
            return rs;
        }

        public override List<FindElementResponse> GetDescribedBy(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_DescribedByPropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var elementArr = element.CachedDescribedBy;
            List<FindElementResponse> rs = [];
            if (elementArr != null)
            {
                for (int i = 0; i < elementArr.Length; i++)
                {
                    try
                    {
                        string itemId = serviceProvider.GetElementFinderService().RegisterElement(elementArr.GetElement(i));
                        rs.Add(new FindElementResponse(itemId));
                    }
                    catch { }
                }
            }
            return rs;
        }

        public override List<FindElementResponse> GetFlowsTo(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_FlowsToPropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var elementArr = element.CachedFlowsTo;
            List<FindElementResponse> rs = [];
            if (elementArr != null)
            {
                for (int i = 0; i < elementArr.Length; i++)
                {
                    try
                    {
                        string itemId = serviceProvider.GetElementFinderService().RegisterElement(elementArr.GetElement(i));
                        rs.Add(new FindElementResponse(itemId));
                    }
                    catch { }
                }
            }
            return rs;
        }

        public override FindElementResponse GetLabeledBy(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_LabeledByPropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            return new FindElementResponse(serviceProvider.GetElementFinderService().RegisterElement(element.CachedLabeledBy));
        }

        public override void SetFocus(string elementId)
        {
            var element = AssertPattern(elementId, automation.CreateCacheRequest());
            element.SetFocus();
        }

        protected override IUIAutomationElement AssertPattern(string elementId, IUIAutomationCacheRequest cacheRequest)
        {
            return serviceProvider.GetElementFinderService().GetElement(elementId, cacheRequest);
        }
    }
}
