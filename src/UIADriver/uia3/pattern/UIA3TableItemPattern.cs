using Interop.UIAutomationClient;
using UIADriver.dto.response;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia3.pattern
{
    public class UIA3TableItemPattern : TableItemPatternService<IUIAutomationElement, IUIAutomationCacheRequest>
    {
        protected IUIAutomation automation;
        public UIA3TableItemPattern(ElementFinderService<IUIAutomationElement, IUIAutomationCacheRequest> finderService, ElementAttributeService<IUIAutomationElement> attributeService, IUIAutomation automation) : base(finderService, attributeService)
        {
            this.automation = automation;
        }

        public override List<FindElementResponse> GetColumnHeaderItems(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_TableItemColumnHeaderItemsPropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationTableItemPattern)element.GetCachedPattern(UIA_PatternIds.UIA_TableItemPatternId);
            var elementArr = pattern.GetCachedColumnHeaderItems();
            List<FindElementResponse> rs = [];
            if (elementArr != null)
            {
                for (int i = 0; i < elementArr.Length; i++)
                {
                    try
                    {
                        string itemId = finderService.RegisterElement(elementArr.GetElement(i));
                        rs.Add(new FindElementResponse(itemId));
                    }
                    catch { }
                }
            }
            return rs;
        }

        public override List<FindElementResponse> GetRowHeaderItems(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_TableItemRowHeaderItemsPropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationTableItemPattern)element.GetCachedPattern(UIA_PatternIds.UIA_TableItemPatternId);
            var elementArr = pattern.GetCachedRowHeaderItems();
            List<FindElementResponse> rs = [];
            if (elementArr != null)
            {
                for (int i = 0; i < elementArr.Length; i++)
                {
                    try
                    {
                        string itemId = finderService.RegisterElement(elementArr.GetElement(i));
                        rs.Add(new FindElementResponse(itemId));
                    }
                    catch { }
                }
            }
            return rs;
        }

        protected override IUIAutomationElement AssertPattern(string elementId, IUIAutomationCacheRequest cacheRequest)
        {
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_IsTableItemPatternAvailablePropertyId);
            cacheRequest.AddPattern(UIA_PatternIds.UIA_TableItemPatternId);
            var element = finderService.GetElement(elementId, cacheRequest);
            if (!(bool)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_IsTableItemPatternAvailablePropertyId))
            {
                throw new InvalidArgument("Table item pattern is not available for this element");
            }
            return element;
        }
    }
}
