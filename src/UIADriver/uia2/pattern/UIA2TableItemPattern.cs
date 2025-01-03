using System.Windows.Automation;
using UIADriver.dto.response;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia2.pattern
{
    public class UIA2TableItemPattern : TableItemPatternService<AutomationElement, CacheRequest>
    {
        public UIA2TableItemPattern(ServiceProvider<AutomationElement, CacheRequest> serviceProvider) : base(serviceProvider) { }

        public override List<FindElementResponse> GetColumnHeaderItems(string elementId)
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(TableItemPattern.ColumnHeaderItemsProperty);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (TableItemPattern)element.GetCachedPattern(TableItemPattern.Pattern);
            var elementArr = pattern.Cached.GetColumnHeaderItems();
            List<FindElementResponse> rs = [];
            if (elementArr != null)
            {
                foreach (var e in elementArr)
                {
                    try
                    {
                        var itemId = serviceProvider.GetElementFinderService().RegisterElement(e);
                        rs.Add(new FindElementResponse(itemId));
                    }
                    catch { }
                }
            }
            return rs;
        }

        public override List<FindElementResponse> GetRowHeaderItems(string elementId)
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(TableItemPattern.RowHeaderItemsProperty);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (TableItemPattern)element.GetCachedPattern(TableItemPattern.Pattern);
            var elementArr = pattern.Cached.GetRowHeaderItems();
            List<FindElementResponse> rs = [];
            if (elementArr != null)
            {
                foreach (var e in elementArr)
                {
                    try
                    {
                        var itemId = serviceProvider.GetElementFinderService().RegisterElement(e);
                        rs.Add(new FindElementResponse(itemId));
                    }
                    catch { }
                }
            }
            return rs;
        }

        protected override AutomationElement AssertPattern(string elementId, CacheRequest cacheRequest)
        {
            cacheRequest.Add(AutomationElement.IsTableItemPatternAvailableProperty);
            cacheRequest.Add(TableItemPattern.Pattern);
            var element = serviceProvider.GetElementFinderService().GetElement(elementId, cacheRequest);
            if (!(bool)element.GetCachedPropertyValue(AutomationElement.IsTableItemPatternAvailableProperty))
            {
                throw new InvalidArgument("Table item pattern is not available for this element");
            }
            return element;
        }
    }
}
