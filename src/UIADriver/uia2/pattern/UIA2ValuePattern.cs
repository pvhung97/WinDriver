using System.Windows.Automation;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia2.pattern
{
    public class UIA2ValuePattern : ValuePatternService<AutomationElement, CacheRequest>
    {
        public UIA2ValuePattern(ServiceProvider<AutomationElement, CacheRequest> serviceProvider) : base(serviceProvider) { }

        public override void SetValue(string elementId, string value)
        {
            var element = AssertPattern(elementId, new CacheRequest());
            var pattern = (ValuePattern)element.GetCachedPattern(ValuePattern.Pattern);
            pattern.SetValue(value);
        }

        protected override AutomationElement AssertPattern(string elementId, CacheRequest cacheRequest)
        {
            cacheRequest.Add(AutomationElement.IsValuePatternAvailableProperty);
            cacheRequest.Add(ValuePattern.Pattern);
            var element = serviceProvider.GetElementFinderService().GetElement(elementId, cacheRequest);
            if (!(bool)element.GetCachedPropertyValue(AutomationElement.IsValuePatternAvailableProperty))
            {
                throw new InvalidArgument("Value pattern is not available for this element");
            }
            return element;
        }
    }
}
