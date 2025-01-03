using Interop.UIAutomationClient;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia3.pattern
{
    public class UIA3ScrollPattern : ScrollPatternService<IUIAutomationElement, IUIAutomationCacheRequest>
    {
        protected IUIAutomation automation;
        public UIA3ScrollPattern(ServiceProvider<IUIAutomationElement, IUIAutomationCacheRequest> serviceProvider, IUIAutomation automation) : base(serviceProvider)
        {
            this.automation = automation;
        }

        protected ScrollAmount validateScrollAmount(string amount)
        {
            switch (amount)
            {
                case "largedecrement":
                    return ScrollAmount.ScrollAmount_LargeDecrement;
                case "smalldecrement":
                    return ScrollAmount.ScrollAmount_SmallDecrement;
                case "noamount":
                    return ScrollAmount.ScrollAmount_NoAmount;
                case "largeincrement":
                    return ScrollAmount.ScrollAmount_LargeIncrement;
                case "smallincrement":
                    return ScrollAmount.ScrollAmount_SmallIncrement;
                default:
                    throw new InvalidArgument("Invalid scroll amount");
            }
        }

        public override void Scroll(string elementId, string horizontalAmount, string verticalAmount)
        {
            var horizontalScroll = validateScrollAmount(horizontalAmount);
            var verticalScroll = validateScrollAmount(verticalAmount);

            var element = AssertPattern(elementId, automation.CreateCacheRequest());
            var pattern = (IUIAutomationScrollPattern)element.GetCachedPattern(UIA_PatternIds.UIA_ScrollPatternId);
            pattern.Scroll(horizontalScroll, verticalScroll);
        }

        public override void SetScrollPercent(string elementId, double horizontalPercent, double verticalPercent)
        {
            var element = AssertPattern(elementId, automation.CreateCacheRequest());
            var pattern = (IUIAutomationScrollPattern)element.GetCachedPattern(UIA_PatternIds.UIA_ScrollPatternId);
            pattern.SetScrollPercent(horizontalPercent, verticalPercent);
        }

        protected override IUIAutomationElement AssertPattern(string elementId, IUIAutomationCacheRequest cacheRequest)
        {
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_IsScrollPatternAvailablePropertyId);
            cacheRequest.AddPattern(UIA_PatternIds.UIA_ScrollPatternId);
            var element = serviceProvider.GetElementFinderService().GetElement(elementId, cacheRequest);
            if (!(bool)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_IsScrollPatternAvailablePropertyId))
            {
                throw new InvalidArgument("Scroll pattern is not available for this element");
            }
            return element;
        }
    }
}
