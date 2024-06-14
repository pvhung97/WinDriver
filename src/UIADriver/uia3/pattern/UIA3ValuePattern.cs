using Interop.UIAutomationClient;
using System.Windows.Automation;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia3.pattern
{
    public class UIA3ValuePattern : ValuePatternService<IUIAutomationElement, IUIAutomationCacheRequest>
    {
        protected IUIAutomation automation;
        public UIA3ValuePattern(ElementFinderService<IUIAutomationElement, IUIAutomationCacheRequest> finderService, ElementAttributeService<IUIAutomationElement> attributeService, IUIAutomation automation) : base(finderService, attributeService)
        {
            this.automation = automation;
        }

        public override void SetValue(string elementId, string value)
        {
            var element = AssertPattern(elementId, automation.CreateCacheRequest());
            var pattern = (IUIAutomationValuePattern)element.GetCachedPattern(UIA_PatternIds.UIA_ValuePatternId);
            pattern.SetValue(value);
        }

        protected override IUIAutomationElement AssertPattern(string elementId, IUIAutomationCacheRequest cacheRequest)
        {
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_IsValuePatternAvailablePropertyId);
            cacheRequest.AddPattern(UIA_PatternIds.UIA_ValuePatternId);
            var element = finderService.GetElement(elementId, cacheRequest);
            if (!(bool)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_IsValuePatternAvailablePropertyId))
            {
                throw new InvalidArgument("Value pattern is not available for this element");
            }
            return element;
        }
    }
}
