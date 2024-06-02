using Interop.UIAutomationClient;
using UIADriver.dto.response;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia3.pattern
{
    public class UIA3SelectionItemPattern : SelectionItemPatternService<IUIAutomationElement, IUIAutomationCacheRequest>
    {
        protected IUIAutomation automation;
        public UIA3SelectionItemPattern(ElementFinderService<IUIAutomationElement, IUIAutomationCacheRequest> finderService, ElementAttributeService<IUIAutomationElement> attributeService, IUIAutomation automation) : base(finderService, attributeService)
        {
            this.automation = automation;
        }

        public override void AddToSelection(string elementId)
        {
            var element = AssertPattern(elementId, automation.CreateCacheRequest());
            var pattern = (IUIAutomationSelectionItemPattern)element.GetCachedPattern(UIA_PatternIds.UIA_SelectionItemPatternId);
            pattern.AddToSelection();
        }

        public override FindElementResponse GetSelectionContainer(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_SelectionItemSelectionContainerPropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationSelectionItemPattern)element.GetCachedPattern(UIA_PatternIds.UIA_SelectionItemPatternId);
            return new FindElementResponse(finderService.RegisterElement(pattern.CachedSelectionContainer));
        }

        public override bool IsSelected(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_SelectionItemIsSelectedPropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationSelectionItemPattern)element.GetCachedPattern(UIA_PatternIds.UIA_SelectionItemPatternId);
            return pattern.CachedIsSelected != 0;
        }

        public override void RemoveFromSelection(string elementId)
        {
            var element = AssertPattern(elementId, automation.CreateCacheRequest());
            var pattern = (IUIAutomationSelectionItemPattern)element.GetCachedPattern(UIA_PatternIds.UIA_SelectionItemPatternId);
            pattern.RemoveFromSelection();
        }

        public override void Select(string elementId)
        {
            var element = AssertPattern(elementId, automation.CreateCacheRequest());
            var pattern = (IUIAutomationSelectionItemPattern)element.GetCachedPattern(UIA_PatternIds.UIA_SelectionItemPatternId);
            pattern.Select();
        }

        protected override IUIAutomationElement AssertPattern(string elementId, IUIAutomationCacheRequest cacheRequest)
        {
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_IsSelectionItemPatternAvailablePropertyId);
            cacheRequest.AddPattern(UIA_PatternIds.UIA_SelectionItemPatternId);
            var element = finderService.GetElement(elementId, cacheRequest);
            if (!(bool)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_IsSelectionItemPatternAvailablePropertyId))
            {
                throw new InvalidArgument("Selection item pattern is not available for this element");
            }
            return element;
        }
    }
}
