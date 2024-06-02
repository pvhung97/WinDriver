using System.Windows.Automation;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia2.pattern
{
    public class UIA2SelectionPattern : SelectionPatternService<AutomationElement, CacheRequest>
    {
        public UIA2SelectionPattern(ElementFinderService<AutomationElement, CacheRequest> finderService, ElementAttributeService<AutomationElement> attributeService) : base(finderService, attributeService) { }

        public override bool CanSelectMultiple(string elementId)
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(SelectionPattern.CanSelectMultipleProperty);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (SelectionPattern)element.GetCachedPattern(SelectionPattern.Pattern);
            return pattern.Cached.CanSelectMultiple;
        }

        public override bool IsSelectionRequired(string elementId)
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(SelectionPattern.IsSelectionRequiredProperty);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (SelectionPattern)element.GetCachedPattern(SelectionPattern.Pattern);
            return pattern.Cached.IsSelectionRequired;
        }

        protected override AutomationElement AssertPattern(string elementId, CacheRequest cacheRequest)
        {
            cacheRequest.Add(AutomationElement.IsSelectionPatternAvailableProperty);
            cacheRequest.Add(SelectionPattern.Pattern);
            var element = finderService.GetElement(elementId, cacheRequest);
            if (!(bool)element.GetCachedPropertyValue(AutomationElement.IsSelectionPatternAvailableProperty))
            {
                throw new InvalidArgument("Selection pattern is not available for this element");
            }
            return element;
        }
    }
}
