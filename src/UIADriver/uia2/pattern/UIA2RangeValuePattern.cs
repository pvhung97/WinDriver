using System.Windows.Automation;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia2.pattern
{
    public class UIA2RangeValuePattern : RangeValuePatternService<AutomationElement, CacheRequest>
    {
        public UIA2RangeValuePattern(ElementFinderService<AutomationElement, CacheRequest> finderService, ElementAttributeService<AutomationElement> attributeService) : base(finderService, attributeService) { }

        public override void SetValue(string elementId, double value)
        {
            var element = AssertPattern(elementId, new CacheRequest());
            var pattern = (RangeValuePattern)element.GetCachedPattern(RangeValuePattern.Pattern);
            pattern.SetValue(value);
        }

        protected override AutomationElement AssertPattern(string elementId, CacheRequest cacheRequest)
        {
            cacheRequest.Add(AutomationElement.IsRangeValuePatternAvailableProperty);
            cacheRequest.Add(RangeValuePattern.Pattern);
            var element = finderService.GetElement(elementId, cacheRequest);
            if (!(bool)element.GetCachedPropertyValue(AutomationElement.IsRangeValuePatternAvailableProperty))
            {
                throw new InvalidArgument("Range value pattern is not available for this element");
            }
            return element;
        }
    }
}
