using Interop.UIAutomationClient;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia3.pattern
{
    public class UIA3TransformPattern : TransformPatternService<IUIAutomationElement, IUIAutomationCacheRequest>
    {
        protected IUIAutomation automation;
        public UIA3TransformPattern(ElementFinderService<IUIAutomationElement, IUIAutomationCacheRequest> finderService, ElementAttributeService<IUIAutomationElement> attributeService, IUIAutomation automation) : base(finderService, attributeService)
        {
            this.automation = automation;
        }

        public override void Move(string elementId, double x, double y)
        {
            var element = AssertPattern(elementId, automation.CreateCacheRequest());
            var pattern = (IUIAutomationTransformPattern)element.GetCachedPattern(UIA_PatternIds.UIA_TransformPatternId);
            pattern.Move(x, y);
        }

        public override void Resize(string elementId, double width, double height)
        {
            var element = AssertPattern(elementId, automation.CreateCacheRequest());
            var pattern = (IUIAutomationTransformPattern)element.GetCachedPattern(UIA_PatternIds.UIA_TransformPatternId);
            pattern.Resize(width, height);
        }

        public override void Rotate(string elementId, double degrees)
        {
            var element = AssertPattern(elementId, automation.CreateCacheRequest());
            var pattern = (IUIAutomationTransformPattern)element.GetCachedPattern(UIA_PatternIds.UIA_TransformPatternId);
            pattern.Rotate(degrees);
        }

        protected override IUIAutomationElement AssertPattern(string elementId, IUIAutomationCacheRequest cacheRequest)
        {
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_IsTransformPatternAvailablePropertyId);
            cacheRequest.AddPattern(UIA_PatternIds.UIA_TransformPatternId);
            var element = finderService.GetElement(elementId, cacheRequest);
            if (!(bool)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_IsTransformPatternAvailablePropertyId))
            {
                throw new InvalidArgument("Transform pattern is not available for this element");
            }
            return element;
        }
    }
}
