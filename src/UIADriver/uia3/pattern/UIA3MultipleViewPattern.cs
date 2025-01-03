using Interop.UIAutomationClient;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia3.pattern
{
    public class UIA3MultipleViewPattern : MultipleViewPatternService<IUIAutomationElement, IUIAutomationCacheRequest>
    {
        protected IUIAutomation automation;

        public UIA3MultipleViewPattern(ServiceProvider<IUIAutomationElement, IUIAutomationCacheRequest> serviceProvider, IUIAutomation automation) : base(serviceProvider)
        {
            this.automation = automation;
        }

        public override List<int> GetSupportedViewIds(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_MultipleViewSupportedViewsPropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationMultipleViewPattern)element.GetCachedPattern(UIA_PatternIds.UIA_MultipleViewPatternId);
            return pattern.GetCachedSupportedViews().ToList();
        }

        public override string GetViewName(string elementId, int viewId)
        {
            var element = AssertPattern(elementId, automation.CreateCacheRequest());
            var pattern = (IUIAutomationMultipleViewPattern)element.GetCachedPattern(UIA_PatternIds.UIA_MultipleViewPatternId);
            return pattern.GetViewName(viewId);
        }

        public override void SetCurrentView(string elementId, int viewId)
        {
            var element = AssertPattern(elementId, automation.CreateCacheRequest());
            var pattern = (IUIAutomationMultipleViewPattern)element.GetCachedPattern(UIA_PatternIds.UIA_MultipleViewPatternId);
            pattern.SetCurrentView(viewId);
        }

        protected override IUIAutomationElement AssertPattern(string elementId, IUIAutomationCacheRequest cacheRequest)
        {
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_IsMultipleViewPatternAvailablePropertyId);
            cacheRequest.AddPattern(UIA_PatternIds.UIA_MultipleViewPatternId);
            var element = serviceProvider.GetElementFinderService().GetElement(elementId, cacheRequest);
            if (!(bool)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_IsMultipleViewPatternAvailablePropertyId))
            {
                throw new InvalidArgument("Multiple view pattern is not available for this element");
            }
            return element;
        }
    }
}
