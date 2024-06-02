using Interop.UIAutomationClient;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia3.pattern
{
    public class UIA3SelectionPattern : SelectionPatternService<IUIAutomationElement, IUIAutomationCacheRequest>
    {
        protected IUIAutomation automation;
        public UIA3SelectionPattern(ElementFinderService<IUIAutomationElement, IUIAutomationCacheRequest> finderService, ElementAttributeService<IUIAutomationElement> attributeService, IUIAutomation automation) : base(finderService, attributeService)
        {
            this.automation = automation;
        }

        public override bool CanSelectMultiple(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_SelectionCanSelectMultiplePropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationSelectionPattern)element.GetCachedPattern(UIA_PatternIds.UIA_SelectionPatternId);
            return pattern.CachedCanSelectMultiple != 0;
        }

        public override bool IsSelectionRequired(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_SelectionIsSelectionRequiredPropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationSelectionPattern)element.GetCachedPattern(UIA_PatternIds.UIA_SelectionPatternId);
            return pattern.CachedIsSelectionRequired != 0;
        }

        protected override IUIAutomationElement AssertPattern(string elementId, IUIAutomationCacheRequest cacheRequest)
        {
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_IsSelectionPatternAvailablePropertyId);
            cacheRequest.AddPattern(UIA_PatternIds.UIA_SelectionPatternId);
            var element = finderService.GetElement(elementId, cacheRequest);
            if (!(bool)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_IsSelectionPatternAvailablePropertyId))
            {
                throw new InvalidArgument("Selection pattern is not available for this element");
            }
            return element;
        }
    }
}
