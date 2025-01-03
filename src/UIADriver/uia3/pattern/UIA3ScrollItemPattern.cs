using Interop.UIAutomationClient;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia3.pattern
{
    public class UIA3ScrollItemPattern : ScrollItemPatternService<IUIAutomationElement, IUIAutomationCacheRequest>
    {
        protected IUIAutomation automation;
        public UIA3ScrollItemPattern(ServiceProvider<IUIAutomationElement, IUIAutomationCacheRequest> serviceProvider, IUIAutomation automation) : base(serviceProvider)
        {
            this.automation = automation;
        }

        public override void ScrollIntoView(string elementId)
        {
            var element = AssertPattern(elementId, automation.CreateCacheRequest());
            var pattern = (IUIAutomationScrollItemPattern)element.GetCachedPattern(UIA_PatternIds.UIA_ScrollItemPatternId);
            pattern.ScrollIntoView();
        }

        protected override IUIAutomationElement AssertPattern(string elementId, IUIAutomationCacheRequest cacheRequest)
        {
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_IsScrollItemPatternAvailablePropertyId);
            cacheRequest.AddPattern(UIA_PatternIds.UIA_ScrollItemPatternId);
            var element = serviceProvider.GetElementFinderService().GetElement(elementId, cacheRequest);
            if (!(bool)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_IsScrollItemPatternAvailablePropertyId))
            {
                throw new InvalidArgument("Scroll item pattern is not available for this element");
            }
            return element;
        }
    }
}
