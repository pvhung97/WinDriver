using System.Windows.Automation;
using UIADriver.dto.response;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia2.pattern
{
    public class UIA2GridPattern : GridPatternService<AutomationElement, CacheRequest>
    {
        public UIA2GridPattern(ElementFinderService<AutomationElement, CacheRequest> finderService, ElementAttributeService<AutomationElement> attributeService) : base(finderService, attributeService) { }

        public override FindElementResponse GetItem(string elementId, int row, int column)
        {
            var cacheRequest = new CacheRequest();
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (GridPattern)element.GetCachedPattern(GridPattern.Pattern);
            FindElementResponse? rsp = null;
            try
            {
                var itemId = finderService.RegisterElement(pattern.GetItem(row, column));
                rsp = new FindElementResponse(itemId);
            } catch { }
            if (rsp == null) throw new NoSuchElement($"No item found with row {row} column {column}");
            return rsp;
        }

        protected override AutomationElement AssertPattern(string elementId, CacheRequest cacheRequest)
        {
            cacheRequest.Add(AutomationElement.IsGridPatternAvailableProperty);
            cacheRequest.Add(GridPattern.Pattern);
            var element = finderService.GetElement(elementId, cacheRequest);
            if (!(bool)element.GetCachedPropertyValue(AutomationElement.IsGridPatternAvailableProperty))
            {
                throw new InvalidArgument("Grid pattern is not available for this element");
            }
            return element;
        }
    }
}
