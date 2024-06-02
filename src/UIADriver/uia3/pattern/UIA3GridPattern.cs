﻿using Interop.UIAutomationClient;
using UIADriver.dto.response;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia3.pattern
{
    public class UIA3GridPattern : GridPatternService<IUIAutomationElement, IUIAutomationCacheRequest>
    {
        protected IUIAutomation automation;

        public UIA3GridPattern(ElementFinderService<IUIAutomationElement, IUIAutomationCacheRequest> finderService, ElementAttributeService<IUIAutomationElement> attributeService, IUIAutomation automation) : base(finderService, attributeService)
        {
            this.automation = automation;
        }

        protected override IUIAutomationElement AssertPattern(string elementId, IUIAutomationCacheRequest cacheRequest)
        {
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_IsGridPatternAvailablePropertyId);
            cacheRequest.AddPattern(UIA_PatternIds.UIA_GridPatternId);
            var element = finderService.GetElement(elementId, cacheRequest);
            if (!(bool)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_IsGridPatternAvailablePropertyId))
            {
                throw new InvalidArgument("Grid pattern is not available for this element");
            }
            return element;
        }

        public override int GetColumnCount(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_GridColumnCountPropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationGridPattern)element.GetCachedPattern(UIA_PatternIds.UIA_GridPatternId);
            return pattern.CachedColumnCount;
        }

        public override int GetRowCount(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_GridRowCountPropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationGridPattern)element.GetCachedPattern(UIA_PatternIds.UIA_GridPatternId);
            return pattern.CachedRowCount;
        }

        public override FindElementResponse GetItem(string elementId, int row, int column)
        {
            var cacheRequest = automation.CreateCacheRequest();
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationGridPattern)element.GetCachedPattern(UIA_PatternIds.UIA_GridPatternId);
            FindElementResponse? rsp = null;
            try
            {
                var itemId = finderService.RegisterElement(pattern.GetItem(row, column));
                rsp = new FindElementResponse(itemId);
            } catch { }
            if (rsp == null) throw new NoSuchElement($"No item found with row {row} column {column}");
            return rsp;
        }
    }
}
