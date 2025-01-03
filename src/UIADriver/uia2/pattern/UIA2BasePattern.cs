using System.Windows.Automation;
using UIADriver.dto.response;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia2.pattern
{
    public class UIA2BasePattern : BasePatternService<AutomationElement, CacheRequest>
    {
        public UIA2BasePattern(ServiceProvider<AutomationElement, CacheRequest> serviceProvider) : base(serviceProvider) { }

        public override List<FindElementResponse> GetControllerFor(string elementId)
        {
            throw new UnsupportedOperation("Not supported in UIA2");
        }

        public override List<FindElementResponse> GetDescribedBy(string elementId)
        {
            throw new UnsupportedOperation("Not supported in UIA2");
        }

        public override List<FindElementResponse> GetFlowsTo(string elementId)
        {
            throw new UnsupportedOperation("Not supported in UIA2");
        }

        public override FindElementResponse GetLabeledBy(string elementId)
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(AutomationElement.LabeledByProperty);
            var element = AssertPattern(elementId, cacheRequest);
            return new FindElementResponse(serviceProvider.GetElementFinderService().RegisterElement(element.Cached.LabeledBy));
        }

        public override void SetFocus(string elementId)
        {
            var element = AssertPattern(elementId, new CacheRequest());
            element.SetFocus();
        }

        protected override AutomationElement AssertPattern(string elementId, CacheRequest cacheRequest)
        {
            return serviceProvider.GetElementFinderService().GetElement(elementId, cacheRequest);
        }
    }
}
