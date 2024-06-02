using System.Windows.Automation;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia2.pattern
{
    public class UIA2ValuePattern : ValuePatternService<AutomationElement, CacheRequest>
    {
        public UIA2ValuePattern(ElementFinderService<AutomationElement, CacheRequest> finderService, ElementAttributeService<AutomationElement> attributeService) : base(finderService, attributeService) { }

        public override string GetValue(string elementId)
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(ValuePattern.ValueProperty);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (ValuePattern)element.GetCachedPattern(ValuePattern.Pattern);
            return pattern.Cached.Value;
        }

        public override bool IsReadOnly(string elementId)
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(ValuePattern.IsReadOnlyProperty);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (ValuePattern)element.GetCachedPattern(ValuePattern.Pattern);
            return pattern.Cached.IsReadOnly;
        }

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
            var element = finderService.GetElement(elementId, cacheRequest);
            if (!(bool)element.GetCachedPropertyValue(AutomationElement.IsValuePatternAvailableProperty))
            {
                throw new InvalidArgument("Value pattern is not available for this element");
            }
            return element;
        }
    }
}
