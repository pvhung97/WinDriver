using Interop.UIAutomationClient;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia3.pattern
{
    public class UIA3RangeValuePattern : RangeValuePatternService<IUIAutomationElement, IUIAutomationCacheRequest>
    {
        protected IUIAutomation automation;
        public UIA3RangeValuePattern(ElementFinderService<IUIAutomationElement, IUIAutomationCacheRequest> finderService, ElementAttributeService<IUIAutomationElement> attributeService, IUIAutomation automation) : base(finderService, attributeService)
        {
            this.automation = automation;
        }

        public override double GetLargeChange(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_RangeValueLargeChangePropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationRangeValuePattern)element.GetCachedPattern(UIA_PatternIds.UIA_RangeValuePatternId);
            return pattern.CachedLargeChange;
        }

        public override double GetMaximum(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_RangeValueMaximumPropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationRangeValuePattern)element.GetCachedPattern(UIA_PatternIds.UIA_RangeValuePatternId);
            return pattern.CachedMaximum;
        }

        public override double GetMinimum(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_RangeValueMinimumPropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationRangeValuePattern)element.GetCachedPattern(UIA_PatternIds.UIA_RangeValuePatternId);
            return pattern.CachedMinimum;
        }

        public override double GetSmallChange(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_RangeValueSmallChangePropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationRangeValuePattern)element.GetCachedPattern(UIA_PatternIds.UIA_RangeValuePatternId);
            return pattern.CachedSmallChange;
        }

        public override double GetValue(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_RangeValueValuePropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationRangeValuePattern)element.GetCachedPattern(UIA_PatternIds.UIA_RangeValuePatternId);
            return pattern.CachedValue;
        }

        public override bool IsReadOnly(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_RangeValueIsReadOnlyPropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationRangeValuePattern)element.GetCachedPattern(UIA_PatternIds.UIA_RangeValuePatternId);
            return pattern.CachedIsReadOnly != 0;
        }

        public override void SetValue(string elementId, double value)
        {
            var element = AssertPattern(elementId, automation.CreateCacheRequest());
            var pattern = (IUIAutomationRangeValuePattern)element.GetCachedPattern(UIA_PatternIds.UIA_RangeValuePatternId);
            pattern.SetValue(value);
        }

        protected override IUIAutomationElement AssertPattern(string elementId, IUIAutomationCacheRequest cacheRequest)
        {
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_IsRangeValuePatternAvailablePropertyId);
            cacheRequest.AddPattern(UIA_PatternIds.UIA_RangeValuePatternId);
            var element = finderService.GetElement(elementId, cacheRequest);
            if (!(bool)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_IsRangeValuePatternAvailablePropertyId))
            {
                throw new InvalidArgument("Range value pattern is not available for this element");
            }
            return element;
        }
    }
}
