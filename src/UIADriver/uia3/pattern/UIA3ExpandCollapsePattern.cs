using Interop.UIAutomationClient;
using System.Runtime.Intrinsics.Arm;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia3.pattern
{
    public class UIA3ExpandCollapsePattern : ExpandCollapsePatternService<IUIAutomationElement, IUIAutomationCacheRequest>
    {
        protected IUIAutomation automation;
        public UIA3ExpandCollapsePattern(ServiceProvider<IUIAutomationElement, IUIAutomationCacheRequest> serviceProvider, IUIAutomation automation) : base(serviceProvider)
        {
            this.automation = automation;
        }

        protected override IUIAutomationElement AssertPattern(string elementId, IUIAutomationCacheRequest cacheRequest)
        {
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_IsExpandCollapsePatternAvailablePropertyId);
            cacheRequest.AddPattern(UIA_PatternIds.UIA_ExpandCollapsePatternId);
            var element = serviceProvider.GetElementFinderService().GetElement(elementId, cacheRequest);
            if (!(bool)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_IsExpandCollapsePatternAvailablePropertyId))
            {
                throw new InvalidArgument("Expand collapse pattern is not available for this element");
            }
            return element;
        }

        public override void ExpandOrCollapseElement(string elementId, bool expand)
        {
            var element = AssertPattern(elementId, automation.CreateCacheRequest());
            var pattern = (IUIAutomationExpandCollapsePattern)element.GetCachedPattern(UIA_PatternIds.UIA_ExpandCollapsePatternId);
            if (expand) pattern.Expand();
            else pattern.Collapse();
        }
    }
}
