using Interop.UIAutomationClient;
using UIADriver.dto.response;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia3.pattern
{
    public class UIA3GridItemPattern : GridItemPatternService<IUIAutomationElement, IUIAutomationCacheRequest>
    {
        protected IUIAutomation automation;
        public UIA3GridItemPattern(ElementFinderService<IUIAutomationElement, IUIAutomationCacheRequest> finderService, ElementAttributeService<IUIAutomationElement> attributeService, IUIAutomation automation) : base(finderService, attributeService)
        {
            this.automation = automation;
        }

        protected override IUIAutomationElement AssertPattern(string elementId, IUIAutomationCacheRequest cacheRequest)
        {
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_IsGridItemPatternAvailablePropertyId);
            cacheRequest.AddPattern(UIA_PatternIds.UIA_GridItemPatternId);
            var element = finderService.GetElement(elementId, cacheRequest);
            if (!(bool)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_IsGridItemPatternAvailablePropertyId))
            {
                throw new InvalidArgument("Grid item pattern is not available for this element");
            }
            return element;
        }

        public override int GetRow(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_GridItemRowPropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationGridItemPattern)element.GetCachedPattern(UIA_PatternIds.UIA_GridItemPatternId);
            return pattern.CachedRow;
        }

        public override int GetColumn(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_GridItemColumnPropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationGridItemPattern)element.GetCachedPattern(UIA_PatternIds.UIA_GridItemPatternId);
            return pattern.CachedColumn;
        }

        public override int GetRowSpan(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_GridItemRowSpanPropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationGridItemPattern)element.GetCachedPattern(UIA_PatternIds.UIA_GridItemPatternId);
            return pattern.CachedRowSpan;
        }

        public override int GetColSpan(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_GridItemColumnSpanPropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationGridItemPattern)element.GetCachedPattern(UIA_PatternIds.UIA_GridItemPatternId);
            return pattern.CachedColumnSpan;
        }

        public override FindElementResponse GetContainingGrid(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_GridItemContainingGridPropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationGridItemPattern)element.GetCachedPattern(UIA_PatternIds.UIA_GridItemPatternId);
            string gridId = finderService.RegisterElement(pattern.CachedContainingGrid);
            return new FindElementResponse(gridId);
        }
    }
}
