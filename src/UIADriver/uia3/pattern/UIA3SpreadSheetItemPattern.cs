using Interop.UIAutomationClient;
using UIADriver.dto.response;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia3.pattern
{
    public class UIA3SpreadSheetItemPattern : SpreadSheetItemPatternService<IUIAutomationElement, IUIAutomationCacheRequest>
    {
        protected IUIAutomation automation;
        public UIA3SpreadSheetItemPattern(ServiceProvider<IUIAutomationElement, IUIAutomationCacheRequest> serviceProvider, IUIAutomation automation) : base(serviceProvider)
        {
            this.automation = automation;
        }

        public override List<FindElementResponse> GetAnnotationObjects(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_SpreadsheetItemAnnotationObjectsPropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationSpreadsheetItemPattern)element.GetCachedPattern(UIA_PatternIds.UIA_SpreadsheetItemPatternId);
            var elementArr = pattern.GetCachedAnnotationObjects();
            List<FindElementResponse> rs = [];
            if (elementArr != null)
            {
                for (int i = 0; i < elementArr.Length; i++)
                {
                    try
                    {
                        string itemId = serviceProvider.GetElementFinderService().RegisterElement(elementArr.GetElement(i));
                        rs.Add(new FindElementResponse(itemId));
                    }
                    catch { }
                }
            }
            return rs;
        }

        protected override IUIAutomationElement AssertPattern(string elementId, IUIAutomationCacheRequest cacheRequest)
        {
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_IsSpreadsheetItemPatternAvailablePropertyId);
            cacheRequest.AddPattern(UIA_PatternIds.UIA_SpreadsheetItemPatternId);
            var element = serviceProvider.GetElementFinderService().GetElement(elementId, cacheRequest);
            if (!(bool)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_IsSpreadsheetItemPatternAvailablePropertyId))
            {
                throw new InvalidArgument("Spread sheet item pattern is not available for this element");
            }
            return element;
        }
    }
}
