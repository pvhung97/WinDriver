using System.Windows.Automation;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia2.pattern
{
    public class UIA2ExpandCollapsePattern : ExpandCollapsePatternService<AutomationElement, CacheRequest>
    {
        public UIA2ExpandCollapsePattern(ElementFinderService<AutomationElement, CacheRequest> finderService, ElementAttributeService<AutomationElement> attributeService) : base(finderService, attributeService) { }

        public override void ExpandOrCollapseElement(string elementId, bool expand)
        {
            var element = AssertPattern(elementId, new CacheRequest());
            var pattern = (ExpandCollapsePattern)element.GetCachedPattern(ExpandCollapsePattern.Pattern);
            if (expand) pattern.Expand();
            else pattern.Collapse();
        }

        protected override AutomationElement AssertPattern(string elementId, CacheRequest cacheRequest)
        {
            cacheRequest.Add(AutomationElement.IsExpandCollapsePatternAvailableProperty);
            cacheRequest.Add(ExpandCollapsePattern.Pattern);
            var element = finderService.GetElement(elementId, cacheRequest);
            if (!(bool)element.GetCachedPropertyValue(AutomationElement.IsExpandCollapsePatternAvailableProperty))
            {
                throw new InvalidArgument("Expand collapse pattern is not available for this element");
            }
            return element;
        }
    }
}
