using Interop.UIAutomationClient;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia3.pattern
{
    public class UIA3ScrollPattern : ScrollPatternService<IUIAutomationElement, IUIAutomationCacheRequest>
    {
        protected IUIAutomation automation;
        public UIA3ScrollPattern(ElementFinderService<IUIAutomationElement, IUIAutomationCacheRequest> finderService, ElementAttributeService<IUIAutomationElement> attributeService, IUIAutomation automation) : base(finderService, attributeService)
        {
            this.automation = automation;
        }

        public override double GetHorizontalScrollPercent(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_ScrollHorizontalScrollPercentPropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationScrollPattern)element.GetCachedPattern(UIA_PatternIds.UIA_ScrollPatternId);
            return pattern.CachedHorizontalScrollPercent;
        }

        public override double GetHorizontalViewSizePercent(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_ScrollHorizontalViewSizePropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationScrollPattern)element.GetCachedPattern(UIA_PatternIds.UIA_ScrollPatternId);
            return pattern.CachedHorizontalViewSize;
        }

        public override double GetVerticalScrollPercent(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_ScrollVerticalScrollPercentPropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationScrollPattern)element.GetCachedPattern(UIA_PatternIds.UIA_ScrollPatternId);
            return pattern.CachedVerticalScrollPercent;
        }

        public override double GetVerticalViewSizePercent(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_ScrollVerticalViewSizePropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationScrollPattern)element.GetCachedPattern(UIA_PatternIds.UIA_ScrollPatternId);
            return pattern.CachedVerticalViewSize;
        }

        public override bool IsHorizontallyScrollable(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_ScrollHorizontallyScrollablePropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationScrollPattern)element.GetCachedPattern(UIA_PatternIds.UIA_ScrollPatternId);
            return pattern.CachedHorizontallyScrollable != 0;
        }

        public override bool IsVerticallyScrollable(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_ScrollVerticallyScrollablePropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationScrollPattern)element.GetCachedPattern(UIA_PatternIds.UIA_ScrollPatternId);
            return pattern.CachedVerticallyScrollable != 0;
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
            var element = finderService.GetElement(elementId, cacheRequest);
            if (!(bool)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_IsScrollPatternAvailablePropertyId))
            {
                throw new InvalidArgument("Scroll pattern is not available for this element");
            }
            return element;
        }
    }
}
