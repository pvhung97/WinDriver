using Interop.UIAutomationClient;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia3.pattern
{
    public class UIA3DropTargetPattern : DropTargetPatternService<IUIAutomationElement, IUIAutomationCacheRequest>
    {
        protected IUIAutomation automation;
        public UIA3DropTargetPattern(ElementFinderService<IUIAutomationElement, IUIAutomationCacheRequest> finderService, ElementAttributeService<IUIAutomationElement> attributeService, IUIAutomation automation) : base(finderService, attributeService)
        {
            this.automation = automation;
        }

        public override string GetDropTargetEffect(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_DropTargetDropTargetEffectPropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationDropTargetPattern)element.GetCachedPattern(UIA_PatternIds.UIA_DropTargetPatternId);
            return pattern.CachedDropTargetEffect;
        }

        public override List<string> GetDropTargetEffects(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_DropTargetDropTargetEffectsPropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationDropTargetPattern)element.GetCachedPattern(UIA_PatternIds.UIA_DropTargetPatternId);
            return pattern.CachedDropTargetEffects.ToList();
        }

        protected override IUIAutomationElement AssertPattern(string elementId, IUIAutomationCacheRequest cacheRequest)
        {
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_IsDropTargetPatternAvailablePropertyId);
            cacheRequest.AddPattern(UIA_PatternIds.UIA_DropTargetPatternId);
            var element = finderService.GetElement(elementId, cacheRequest);
            if (!(bool)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_IsDropTargetPatternAvailablePropertyId))
            {
                throw new InvalidArgument("Drop target pattern is not available for this element");
            }
            return element;
        }
    }
}
