using System.Windows.Automation;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia2.pattern
{
    public class UIA2DockPattern : DockPatternService<AutomationElement, CacheRequest>
    {
        public UIA2DockPattern(ElementFinderService<AutomationElement, CacheRequest> finderService, ElementAttributeService<AutomationElement> attributeService) : base(finderService, attributeService) { }

        public override void SetDockPosition(string elementId, string dockPosition)
        {
            DockPosition dp;
            switch (dockPosition.ToLower())
            {
                case "none":
                    dp = DockPosition.None;
                    break;
                case "left":
                    dp = DockPosition.Left;
                    break;
                case "right":
                    dp = DockPosition.Right;
                    break;
                case "top":
                    dp = DockPosition.Top;
                    break;
                case "bottom":
                    dp = DockPosition.Bottom;
                    break;
                case "fill":
                    dp = DockPosition.Fill;
                    break;
                default:
                    throw new InvalidArgument("Invalid dock position");
            }
            var element = AssertPattern(elementId, new CacheRequest());
            var pattern = (DockPattern) element.GetCachedPattern(DockPattern.Pattern);
            pattern.SetDockPosition(dp);
        }

        protected override AutomationElement AssertPattern(string elementId, CacheRequest cacheRequest)
        {
            cacheRequest.Add(AutomationElement.IsDockPatternAvailableProperty);
            cacheRequest.Add(DockPattern.Pattern);
            var element = finderService.GetElement(elementId, cacheRequest);
            if (!(bool) element.GetCachedPropertyValue(AutomationElement.IsDockPatternAvailableProperty))
            {
                throw new InvalidArgument("Dock pattern is not available for this element");
            }
            return element;
        }
    }
}
