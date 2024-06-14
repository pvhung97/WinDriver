using Interop.UIAutomationClient;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia3.pattern
{
    public class UIA3DockPattern : DockPatternService<IUIAutomationElement, IUIAutomationCacheRequest>
    {
        protected IUIAutomation automation;

        public UIA3DockPattern(ElementFinderService<IUIAutomationElement, IUIAutomationCacheRequest> finderService, ElementAttributeService<IUIAutomationElement> attributeService, IUIAutomation automation) : base(finderService, attributeService)
        {
            this.automation = automation;
        }

        protected override IUIAutomationElement AssertPattern(string elementId, IUIAutomationCacheRequest cacheRequest)
        {
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_IsDockPatternAvailablePropertyId);
            cacheRequest.AddPattern(UIA_PatternIds.UIA_DockPatternId);
            var element = finderService.GetElement(elementId, cacheRequest);
            if (!(bool) element.GetCachedPropertyValue(UIA_PropertyIds.UIA_IsDockPatternAvailablePropertyId))
            {
                throw new InvalidArgument("Dock pattern is not available for this element");
            }
            return element;
        }

        public override void SetDockPosition(string elementId, string dockPosition)
        {
            DockPosition dp = DockPosition.DockPosition_None;
            switch (dockPosition.ToLower())
            {
                case "none":
                    dp = DockPosition.DockPosition_None;
                    break;
                case "left":
                    dp = DockPosition.DockPosition_Left;
                    break;
                case "right":
                    dp = DockPosition.DockPosition_Right;
                    break;
                case "top":
                    dp = DockPosition.DockPosition_Top;
                    break;
                case "bottom":
                    dp = DockPosition.DockPosition_Bottom;
                    break;
                case "fill":
                    dp = DockPosition.DockPosition_Fill;
                    break;
                default:
                    throw new InvalidArgument("Invalid dock position");
            }
            var element = AssertPattern(elementId, automation.CreateCacheRequest());
            var pattern = (IUIAutomationDockPattern) element.GetCachedPattern(UIA_PatternIds.UIA_DockPatternId);
            pattern.SetDockPosition(dp);
        }
    }
}
