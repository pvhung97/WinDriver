using System.Windows.Automation;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia2.pattern
{
    public class UIA2MultipleViewPattern : MultipleViewPatternService<AutomationElement, CacheRequest>
    {
        public UIA2MultipleViewPattern(ServiceProvider<AutomationElement, CacheRequest> serviceProvider) : base(serviceProvider) { }

        public override List<int> GetSupportedViewIds(string elementId)
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(MultipleViewPattern.SupportedViewsProperty);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (MultipleViewPattern)element.GetCachedPattern(MultipleViewPattern.Pattern);
            return pattern.Cached.GetSupportedViews().ToList();
        }

        public override string GetViewName(string elementId, int viewId)
        {
            var element = AssertPattern(elementId, new CacheRequest());
            var pattern = (MultipleViewPattern)element.GetCachedPattern(MultipleViewPattern.Pattern);
            return pattern.GetViewName(viewId);
        }

        public override void SetCurrentView(string elementId, int viewId)
        {
            var element = AssertPattern(elementId, new CacheRequest());
            var pattern = (MultipleViewPattern)element.GetCachedPattern(MultipleViewPattern.Pattern);
            pattern.SetCurrentView(viewId);
        }

        protected override AutomationElement AssertPattern(string elementId, CacheRequest cacheRequest)
        {
            cacheRequest.Add(AutomationElement.IsMultipleViewPatternAvailableProperty);
            cacheRequest.Add(MultipleViewPattern.Pattern);
            var element = serviceProvider.GetElementFinderService().GetElement(elementId, cacheRequest);
            if (!(bool)element.GetCachedPropertyValue(AutomationElement.IsMultipleViewPatternAvailableProperty))
            {
                throw new InvalidArgument("Multiple view pattern is not available for this element");
            }
            return element;
        }
    }
}
