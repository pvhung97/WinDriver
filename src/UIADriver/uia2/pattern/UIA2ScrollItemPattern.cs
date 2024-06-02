using System.Windows.Automation;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia2.pattern
{
    public class UIA2ScrollItemPattern : ScrollItemPatternService<AutomationElement, CacheRequest>
    {
        public UIA2ScrollItemPattern(ElementFinderService<AutomationElement, CacheRequest> finderService, ElementAttributeService<AutomationElement> attributeService) : base(finderService, attributeService) { }

        public override void ScrollIntoView(string elementId)
        {
            var element = AssertPattern(elementId, new CacheRequest());
            var pattern = (ScrollItemPattern)element.GetCachedPattern(ScrollItemPattern.Pattern);
            pattern.ScrollIntoView();
        }

        protected override AutomationElement AssertPattern(string elementId, CacheRequest cacheRequest)
        {
            cacheRequest.Add(AutomationElement.IsScrollItemPatternAvailableProperty);
            cacheRequest.Add(ScrollItemPattern.Pattern);
            var element = finderService.GetElement(elementId, cacheRequest);
            if (!(bool)element.GetCachedPropertyValue(AutomationElement.IsScrollItemPatternAvailableProperty))
            {
                throw new InvalidArgument("Scroll item pattern is not available for this element");
            }
            return element;
        }
    }
}
