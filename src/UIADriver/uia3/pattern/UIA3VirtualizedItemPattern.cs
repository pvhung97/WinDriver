using Interop.UIAutomationClient;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia3.pattern
{
    public class UIA3VirtualizedItemPattern : VirtualizedItemPatternService<IUIAutomationElement, IUIAutomationCacheRequest>
    {
        protected IUIAutomation automation;
        public UIA3VirtualizedItemPattern(ServiceProvider<IUIAutomationElement, IUIAutomationCacheRequest> serviceProvider, IUIAutomation automation) : base(serviceProvider)
        {
            this.automation = automation;
        }

        public override void Realize(string elementId)
        {
            var element = AssertPattern(elementId, automation.CreateCacheRequest());
            var pattern = (IUIAutomationVirtualizedItemPattern)element.GetCachedPattern(UIA_PatternIds.UIA_VirtualizedItemPatternId);
            pattern.Realize();
        }

        protected override IUIAutomationElement AssertPattern(string elementId, IUIAutomationCacheRequest cacheRequest)
        {
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_IsVirtualizedItemPatternAvailablePropertyId);
            cacheRequest.AddPattern(UIA_PatternIds.UIA_VirtualizedItemPatternId);
            var element = serviceProvider.GetElementFinderService().GetElement(elementId, cacheRequest);
            if (!(bool)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_IsVirtualizedItemPatternAvailablePropertyId))
            {
                throw new InvalidArgument("Virtualized item pattern is not available for this element");
            }
            return element;
        }
    }
}
