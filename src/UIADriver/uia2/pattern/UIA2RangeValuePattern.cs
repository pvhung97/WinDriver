using System.Windows.Automation;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia2.pattern
{
    public class UIA2RangeValuePattern : RangeValuePatternService<AutomationElement, CacheRequest>
    {
        public UIA2RangeValuePattern(ElementFinderService<AutomationElement, CacheRequest> finderService, ElementAttributeService<AutomationElement> attributeService) : base(finderService, attributeService) { }

        public override double GetLargeChange(string elementId)
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(RangeValuePattern.LargeChangeProperty);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (RangeValuePattern)element.GetCachedPattern(RangeValuePattern.Pattern);
            return pattern.Cached.LargeChange;
        }

        public override double GetMaximum(string elementId)
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(RangeValuePattern.MaximumProperty);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (RangeValuePattern)element.GetCachedPattern(RangeValuePattern.Pattern);
            return pattern.Cached.Maximum;
        }

        public override double GetMinimum(string elementId)
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(RangeValuePattern.MinimumProperty);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (RangeValuePattern)element.GetCachedPattern(RangeValuePattern.Pattern);
            return pattern.Cached.Minimum;
        }

        public override double GetSmallChange(string elementId)
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(RangeValuePattern.SmallChangeProperty);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (RangeValuePattern)element.GetCachedPattern(RangeValuePattern.Pattern);
            return pattern.Cached.SmallChange;
        }

        public override double GetValue(string elementId)
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(RangeValuePattern.ValueProperty);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (RangeValuePattern)element.GetCachedPattern(RangeValuePattern.Pattern);
            return pattern.Cached.Value;
        }

        public override bool IsReadOnly(string elementId)
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(RangeValuePattern.IsReadOnlyProperty);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (RangeValuePattern)element.GetCachedPattern(RangeValuePattern.Pattern);
            return pattern.Cached.IsReadOnly;
        }

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
