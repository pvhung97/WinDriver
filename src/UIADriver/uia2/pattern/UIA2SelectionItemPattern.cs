using System.Windows.Automation;
using UIADriver.dto.response;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia2.pattern
{
    public class UIA2SelectionItemPattern : SelectionItemPatternService<AutomationElement, CacheRequest>
    {
        public UIA2SelectionItemPattern(ElementFinderService<AutomationElement, CacheRequest> finderService, ElementAttributeService<AutomationElement> attributeService) : base(finderService, attributeService) { }

        public override void AddToSelection(string elementId)
        {
            var element = AssertPattern(elementId, new CacheRequest());
            var pattern = (SelectionItemPattern)element.GetCachedPattern(SelectionItemPattern.Pattern);
            pattern.AddToSelection();
        }

        public override FindElementResponse GetSelectionContainer(string elementId)
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(SelectionItemPattern.SelectionContainerProperty);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (SelectionItemPattern)element.GetCachedPattern(SelectionItemPattern.Pattern);
            return new FindElementResponse(finderService.RegisterElement(pattern.Cached.SelectionContainer));
        }

        public override bool IsSelected(string elementId)
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(SelectionItemPattern.IsSelectedProperty);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (SelectionItemPattern)element.GetCachedPattern(SelectionItemPattern.Pattern);
            return pattern.Cached.IsSelected;
        }

        public override void RemoveFromSelection(string elementId)
        {
            var element = AssertPattern(elementId, new CacheRequest());
            var pattern = (SelectionItemPattern)element.GetCachedPattern(SelectionItemPattern.Pattern);
            pattern.RemoveFromSelection();
        }

        public override void Select(string elementId)
        {
            var element = AssertPattern(elementId, new CacheRequest());
            var pattern = (SelectionItemPattern)element.GetCachedPattern(SelectionItemPattern.Pattern);
            pattern.Select();
        }

        protected override AutomationElement AssertPattern(string elementId, CacheRequest cacheRequest)
        {
            cacheRequest.Add(AutomationElement.IsSelectionItemPatternAvailableProperty);
            cacheRequest.Add(SelectionItemPattern.Pattern);
            var element = finderService.GetElement(elementId, cacheRequest);
            if (!(bool)element.GetCachedPropertyValue(AutomationElement.IsSelectionItemPatternAvailableProperty))
            {
                throw new InvalidArgument("Selection item pattern is not available for this element");
            }
            return element;
        }
    }
}
