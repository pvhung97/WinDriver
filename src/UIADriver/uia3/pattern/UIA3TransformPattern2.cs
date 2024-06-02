using Interop.UIAutomationClient;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia3.pattern
{
    public class UIA3TransformPattern2 : TransformPattern2Service<IUIAutomationElement, IUIAutomationCacheRequest>
    {
        protected IUIAutomation automation;
        public UIA3TransformPattern2(ElementFinderService<IUIAutomationElement, IUIAutomationCacheRequest> finderService, ElementAttributeService<IUIAutomationElement> attributeService, IUIAutomation automation) : base(finderService, attributeService)
        {
            this.automation = automation;
        }

        public override bool CanZoom(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_Transform2CanZoomPropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationTransformPattern2)element.GetCachedPattern(UIA_PatternIds.UIA_TransformPattern2Id);
            return pattern.CachedCanZoom != 0;
        }

        public override double GetZoomLevel(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_Transform2ZoomLevelPropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationTransformPattern2)element.GetCachedPattern(UIA_PatternIds.UIA_TransformPattern2Id);
            return pattern.CachedZoomLevel;
        }

        public override double GetZoomMaximum(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_Transform2ZoomMaximumPropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationTransformPattern2)element.GetCachedPattern(UIA_PatternIds.UIA_TransformPattern2Id);
            return pattern.CachedZoomMaximum;
        }

        public override double GetZoomMinimum(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_Transform2ZoomMinimumPropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationTransformPattern2)element.GetCachedPattern(UIA_PatternIds.UIA_TransformPattern2Id);
            return pattern.CachedZoomMinimum;
        }

        public override void Zoom(string elementId, double value)
        {
            var element = AssertPattern(elementId, automation.CreateCacheRequest());
            var pattern = (IUIAutomationTransformPattern2)element.GetCachedPattern(UIA_PatternIds.UIA_TransformPattern2Id);
            pattern.Zoom(value);
        }

        public override void ZoomByUnit(string elementId, string zoomUnit)
        {
            ZoomUnit unit = ZoomUnit.ZoomUnit_NoAmount;
            switch (zoomUnit.ToLower())
            {
                case "noamount":
                    unit = ZoomUnit.ZoomUnit_NoAmount;
                    break;
                case "largedecrement":
                    unit = ZoomUnit.ZoomUnit_LargeDecrement;
                    break;
                case "largeincrement":
                    unit = ZoomUnit.ZoomUnit_LargeIncrement;
                    break;
                case "smalldecrement":
                    unit = ZoomUnit.ZoomUnit_SmallDecrement;
                    break;
                case "smallincrement":
                    unit = ZoomUnit.ZoomUnit_SmallIncrement;
                    break;
                default:
                    throw new InvalidArgument("Invalid zoom unit");
            }
            var element = AssertPattern(elementId, automation.CreateCacheRequest());
            var pattern = (IUIAutomationTransformPattern2)element.GetCachedPattern(UIA_PatternIds.UIA_TransformPattern2Id);
            pattern.ZoomByUnit(unit);
        }

        protected override IUIAutomationElement AssertPattern(string elementId, IUIAutomationCacheRequest cacheRequest)
        {
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_IsTransformPattern2AvailablePropertyId);
            cacheRequest.AddPattern(UIA_PatternIds.UIA_TransformPattern2Id);
            var element = finderService.GetElement(elementId, cacheRequest);
            if (!(bool)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_IsTransformPattern2AvailablePropertyId))
            {
                throw new InvalidArgument("Transform pattern 2 is not available for this element");
            }
            return element;
        }
    }
}
