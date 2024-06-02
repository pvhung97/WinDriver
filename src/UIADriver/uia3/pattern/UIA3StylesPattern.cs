using Interop.UIAutomationClient;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia3.pattern
{
    public class UIA3StylesPattern : StylesPatternService<IUIAutomationElement, IUIAutomationCacheRequest>
    {
        protected IUIAutomation automation;
        public UIA3StylesPattern(ElementFinderService<IUIAutomationElement, IUIAutomationCacheRequest> finderService, ElementAttributeService<IUIAutomationElement> attributeService, IUIAutomation automation) : base(finderService, attributeService)
        {
            this.automation = automation;
        }

        public override string GetExtendedProperties(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_StylesExtendedPropertiesPropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationStylesPattern)element.GetCachedPattern(UIA_PatternIds.UIA_StylesPatternId);
            return pattern.CachedExtendedProperties;
        }

        public override int GetFillColor(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_StylesFillColorPropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationStylesPattern)element.GetCachedPattern(UIA_PatternIds.UIA_StylesPatternId);
            return pattern.CachedFillColor;
        }

        public override int GetFillPatternColor(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_StylesFillPatternColorPropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationStylesPattern)element.GetCachedPattern(UIA_PatternIds.UIA_StylesPatternId);
            return pattern.CachedFillPatternColor;
        }

        public override string GetFillPatternStyle(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_StylesFillPatternStylePropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationStylesPattern)element.GetCachedPattern(UIA_PatternIds.UIA_StylesPatternId);
            return pattern.CachedFillPatternStyle;
        }

        public override string GetShape(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_StylesShapePropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationStylesPattern)element.GetCachedPattern(UIA_PatternIds.UIA_StylesPatternId);
            return pattern.CachedShape;
        }

        public override int GetStyleId(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_StylesStyleIdPropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationStylesPattern)element.GetCachedPattern(UIA_PatternIds.UIA_StylesPatternId);
            return pattern.CachedStyleId;
        }

        public override string GetStyleName(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_StylesStyleNamePropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationStylesPattern)element.GetCachedPattern(UIA_PatternIds.UIA_StylesPatternId);
            return pattern.CachedStyleName;
        }

        protected override IUIAutomationElement AssertPattern(string elementId, IUIAutomationCacheRequest cacheRequest)
        {
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_IsStylesPatternAvailablePropertyId);
            cacheRequest.AddPattern(UIA_PatternIds.UIA_StylesPatternId);
            var element = finderService.GetElement(elementId, cacheRequest);
            if (!(bool)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_IsStylesPatternAvailablePropertyId))
            {
                throw new InvalidArgument("Styles pattern is not available for this element");
            }
            return element;
        }
    }
}
