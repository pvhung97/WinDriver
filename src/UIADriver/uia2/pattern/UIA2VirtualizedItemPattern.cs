using System.Windows.Automation;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia2.pattern
{
    public class UIA2VirtualizedItemPattern : VirtualizedItemPatternService<AutomationElement, CacheRequest>
    {
        public UIA2VirtualizedItemPattern(ElementFinderService<AutomationElement, CacheRequest> finderService, ElementAttributeService<AutomationElement> attributeService) : base(finderService, attributeService) { }

        public override void Realize(string elementId)
        {
            var element = AssertPattern(elementId, new CacheRequest());
            var pattern = (VirtualizedItemPattern)element.GetCachedPattern(VirtualizedItemPattern.Pattern);
            pattern.Realize();
        }

        protected override AutomationElement AssertPattern(string elementId, CacheRequest cacheRequest)
        {
            cacheRequest.Add(AutomationElement.IsVirtualizedItemPatternAvailableProperty);
            cacheRequest.Add(VirtualizedItemPattern.Pattern);
            var element = finderService.GetElement(elementId, cacheRequest);
            if (!(bool)element.GetCachedPropertyValue(AutomationElement.IsVirtualizedItemPatternAvailableProperty))
            {
                throw new InvalidArgument("Virtualized item pattern is not available for this element");
            }
            return element;
        }
    }
}
