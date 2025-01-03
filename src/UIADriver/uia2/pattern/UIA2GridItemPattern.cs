using System.Windows.Automation;
using UIADriver.dto.response;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia2.pattern
{
    public class UIA2GridItemPattern : GridItemPatternService<AutomationElement, CacheRequest>
    {
        public UIA2GridItemPattern(ServiceProvider<AutomationElement, CacheRequest> serviceProvider) : base(serviceProvider) { }

        public override FindElementResponse GetContainingGrid(string elementId)
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(GridItemPattern.ContainingGridProperty);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (GridItemPattern)element.GetCachedPattern(GridItemPattern.Pattern);
            var gridId = serviceProvider.GetElementFinderService().RegisterElement(pattern.Cached.ContainingGrid);
            return new FindElementResponse(gridId);
        }

        protected override AutomationElement AssertPattern(string elementId, CacheRequest cacheRequest)
        {
            cacheRequest.Add(AutomationElement.IsGridItemPatternAvailableProperty);
            cacheRequest.Add(GridItemPattern.Pattern);
            var element = serviceProvider.GetElementFinderService().GetElement(elementId, cacheRequest);
            if (!(bool)element.GetCachedPropertyValue(AutomationElement.IsGridItemPatternAvailableProperty))
            {
                throw new InvalidArgument("Grid item pattern is not available for this element");
            }
            return element;
        }
    }
}
