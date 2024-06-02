using Interop.UIAutomationClient;
using UIADriver.dto.response;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia3.pattern
{
    public class UIA3SpreadSheetPattern : SpreadSheetPatternService<IUIAutomationElement, IUIAutomationCacheRequest>
    {
        protected IUIAutomation automation;
        public UIA3SpreadSheetPattern(ElementFinderService<IUIAutomationElement, IUIAutomationCacheRequest> finderService, ElementAttributeService<IUIAutomationElement> attributeService, IUIAutomation automation) : base(finderService, attributeService)
        {
            this.automation = automation;
        }

        public override FindElementResponse GetItemByName(string elementId, string name)
        {
            var element = AssertPattern(elementId, automation.CreateCacheRequest());
            var pattern = (IUIAutomationSpreadsheetPattern)element.GetCachedPattern(UIA_PatternIds.UIA_SpreadsheetPatternId);
            FindElementResponse? rsp = null;
            try
            {
                var itemId = finderService.RegisterElement(pattern.GetItemByName(name));
                rsp = new FindElementResponse(itemId);
            }
            catch { }
            if (rsp == null) throw new NoSuchElement($"No item found with name {name}");
            return rsp;
        }

        protected override IUIAutomationElement AssertPattern(string elementId, IUIAutomationCacheRequest cacheRequest)
        {
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_IsSpreadsheetPatternAvailablePropertyId);
            cacheRequest.AddPattern(UIA_PatternIds.UIA_SpreadsheetPatternId);
            var element = finderService.GetElement(elementId, cacheRequest);
            if (!(bool)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_IsSpreadsheetPatternAvailablePropertyId))
            {
                throw new InvalidArgument("Spread sheet pattern is not available for this element");
            }
            return element;
        }
    }
}
