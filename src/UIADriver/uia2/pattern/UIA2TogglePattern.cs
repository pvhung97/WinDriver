using System.Windows.Automation;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia2.pattern
{
    public class UIA2TogglePattern : TogglePatternService<AutomationElement, CacheRequest>
    {
        public UIA2TogglePattern(ServiceProvider<AutomationElement, CacheRequest> serviceProvider) : base(serviceProvider) { }

        public override void Toggle(string elementId)
        {
            var element = AssertPattern(elementId, new CacheRequest());
            var pattern = (TogglePattern)element.GetCachedPattern(TogglePattern.Pattern);
            pattern.Toggle();
        }

        protected override AutomationElement AssertPattern(string elementId, CacheRequest cacheRequest)
        {
            cacheRequest.Add(AutomationElement.IsTogglePatternAvailableProperty);
            cacheRequest.Add(TogglePattern.Pattern);
            var element = serviceProvider.GetElementFinderService().GetElement(elementId, cacheRequest);
            if (!(bool)element.GetCachedPropertyValue(AutomationElement.IsTogglePatternAvailableProperty))
            {
                throw new InvalidArgument("Toggle pattern is not available for this element");
            }
            return element;
        }
    }
}
