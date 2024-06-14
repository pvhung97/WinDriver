using System.Windows.Automation;
using UIADriver.dto.response;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia2.pattern
{
    public class UIA2TablePattern : TablePatternService<AutomationElement, CacheRequest>
    {
        public UIA2TablePattern(ElementFinderService<AutomationElement, CacheRequest> finderService, ElementAttributeService<AutomationElement> attributeService) : base(finderService, attributeService) { }

        public override List<FindElementResponse> GetColumnHeaders(string elementId)
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(TablePattern.ColumnHeadersProperty);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (TablePattern)element.GetCachedPattern(TablePattern.Pattern);
            var elementArr = pattern.Cached.GetColumnHeaders();
            List<FindElementResponse> rs = [];
            if (elementArr != null)
            {
                foreach (var e in elementArr)
                {
                    try
                    {
                        var itemId = finderService.RegisterElement(e);
                        rs.Add(new FindElementResponse(itemId));
                     }
                    catch { }
                }
            }
            return rs;
        }

        public override List<FindElementResponse> GetRowHeaders(string elementId)
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(TablePattern.RowHeadersProperty);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (TablePattern)element.GetCachedPattern(TablePattern.Pattern);
            var elementArr = pattern.Cached.GetRowHeaders();
            List<FindElementResponse> rs = [];
            if (elementArr != null)
            {
                foreach (var e in elementArr)
                {
                    try
                    {
                        var itemId = finderService.RegisterElement(e);
                        rs.Add(new FindElementResponse(itemId));
                    }
                    catch { }
                }
            }
            return rs;
        }

        protected override AutomationElement AssertPattern(string elementId, CacheRequest cacheRequest)
        {
            cacheRequest.Add(AutomationElement.IsTablePatternAvailableProperty);
            cacheRequest.Add(TablePattern.Pattern);
            var element = finderService.GetElement(elementId, cacheRequest);
            if (!(bool)element.GetCachedPropertyValue(AutomationElement.IsTablePatternAvailableProperty))
            {
                throw new InvalidArgument("Table pattern is not available for this element");
            }
            return element;
        }
    }
}
