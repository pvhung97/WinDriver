using Interop.UIAutomationClient;
using UIADriver.dto.response;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia3.pattern
{
    public class UIA3SelectionPattern2 : SelectionPattern2Service<IUIAutomationElement, IUIAutomationCacheRequest>
    {
        protected IUIAutomation automation;
        public UIA3SelectionPattern2(ServiceProvider<IUIAutomationElement, IUIAutomationCacheRequest> serviceProvider, IUIAutomation automation) : base(serviceProvider)
        {
            this.automation = automation;
        }

        public override FindElementResponse GetCurrentSelectedItem(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_Selection2CurrentSelectedItemPropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationSelectionPattern2)element.GetCachedPattern(UIA_PatternIds.UIA_SelectionPattern2Id);
            return new FindElementResponse(serviceProvider.GetElementFinderService().RegisterElement(pattern.CachedCurrentSelectedItem));
        }

        public override FindElementResponse GetFirstSelectedItem(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_Selection2FirstSelectedItemPropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationSelectionPattern2)element.GetCachedPattern(UIA_PatternIds.UIA_SelectionPattern2Id);
            return new FindElementResponse(serviceProvider.GetElementFinderService().RegisterElement(pattern.CachedFirstSelectedItem));
        }

        public override FindElementResponse GetLastSelectedItem(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_Selection2LastSelectedItemPropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationSelectionPattern2)element.GetCachedPattern(UIA_PatternIds.UIA_SelectionPattern2Id);
            return new FindElementResponse(serviceProvider.GetElementFinderService().RegisterElement(pattern.CachedLastSelectedItem));
        }

        protected override IUIAutomationElement AssertPattern(string elementId, IUIAutomationCacheRequest cacheRequest)
        {
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_IsSelectionPattern2AvailablePropertyId);
            cacheRequest.AddPattern(UIA_PatternIds.UIA_SelectionPattern2Id);
            var element = serviceProvider.GetElementFinderService().GetElement(elementId, cacheRequest);
            if (!(bool)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_IsSelectionPattern2AvailablePropertyId))
            {
                throw new InvalidArgument("Selection pattern 2 is not available for this element");
            }
            return element;
        }
    }
}
