using Interop.UIAutomationClient;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia3.pattern
{
    public class UIA3InvokePattern : InvokePatternService<IUIAutomationElement, IUIAutomationCacheRequest>
    {
        protected IUIAutomation automation;

        public UIA3InvokePattern(ElementFinderService<IUIAutomationElement, IUIAutomationCacheRequest> finderService, ElementAttributeService<IUIAutomationElement> attributeService, IUIAutomation automation) : base(finderService, attributeService)
        {
            this.automation = automation;
        }

        public override void Invoke(string elementId)
        {
            var element = AssertPattern(elementId, automation.CreateCacheRequest());
            var pattern = (IUIAutomationInvokePattern)element.GetCachedPattern(UIA_PatternIds.UIA_InvokePatternId);
            pattern.Invoke();
        }

        protected override IUIAutomationElement AssertPattern(string elementId, IUIAutomationCacheRequest cacheRequest)
        {
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_IsInvokePatternAvailablePropertyId);
            cacheRequest.AddPattern(UIA_PatternIds.UIA_InvokePatternId);
            var element = finderService.GetElement(elementId, cacheRequest);
            if (!(bool)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_IsInvokePatternAvailablePropertyId))
            {
                throw new InvalidArgument("Invoke pattern is not available for this element");
            }
            return element;
        }
    }
}
