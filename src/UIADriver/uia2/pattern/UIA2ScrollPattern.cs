using System.Windows.Automation;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia2.pattern
{
    public class UIA2ScrollPattern : ScrollPatternService<AutomationElement, CacheRequest>
    {
        public UIA2ScrollPattern(ElementFinderService<AutomationElement, CacheRequest> finderService, ElementAttributeService<AutomationElement> attributeService) : base(finderService, attributeService) { }

        public override double GetHorizontalScrollPercent(string elementId)
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(ScrollPattern.HorizontalScrollPercentProperty);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (ScrollPattern)element.GetCachedPattern(ScrollPattern.Pattern);
            return pattern.Cached.HorizontalScrollPercent;
        }

        public override double GetHorizontalViewSizePercent(string elementId)
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(ScrollPattern.HorizontalViewSizeProperty);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (ScrollPattern)element.GetCachedPattern(ScrollPattern.Pattern);
            return pattern.Cached.HorizontalViewSize;
        }

        public override double GetVerticalScrollPercent(string elementId)
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(ScrollPattern.VerticalScrollPercentProperty);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (ScrollPattern)element.GetCachedPattern(ScrollPattern.Pattern);
            return pattern.Cached.VerticalScrollPercent;
        }

        public override double GetVerticalViewSizePercent(string elementId)
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(ScrollPattern.VerticalViewSizeProperty);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (ScrollPattern)element.GetCachedPattern(ScrollPattern.Pattern);
            return pattern.Cached.VerticalViewSize;
        }

        public override bool IsHorizontallyScrollable(string elementId)
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(ScrollPattern.HorizontallyScrollableProperty);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (ScrollPattern)element.GetCachedPattern(ScrollPattern.Pattern);
            return pattern.Cached.HorizontallyScrollable;
        }

        public override bool IsVerticallyScrollable(string elementId)
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(ScrollPattern.VerticallyScrollableProperty);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (ScrollPattern)element.GetCachedPattern(ScrollPattern.Pattern);
            return pattern.Cached.VerticallyScrollable;
        }

        protected ScrollAmount validateScrollAmount(string amount)
        {
            switch (amount)
            {
                case "largedecrement":
                    return ScrollAmount.LargeDecrement;
                case "smalldecrement":
                    return ScrollAmount.SmallDecrement;
                case "noamount":
                    return ScrollAmount.NoAmount;
                case "largeincrement":
                    return ScrollAmount.LargeIncrement;
                case "smallincrement":
                    return ScrollAmount.SmallIncrement;
                default:
                    throw new InvalidArgument("Invalid scroll amount");
            }
        }

        public override void Scroll(string elementId, string horizontalAmount, string verticalAmount)
        {
            var horizontalScroll = validateScrollAmount(horizontalAmount);
            var verticalScroll = validateScrollAmount(verticalAmount);

            var element = AssertPattern(elementId, new CacheRequest());
            var pattern = (ScrollPattern)element.GetCachedPattern(ScrollPattern.Pattern);
            pattern.Scroll(horizontalScroll, verticalScroll);
        }

        public override void SetScrollPercent(string elementId, double horizontalPercent, double verticalPercent)
        {
            var element = AssertPattern(elementId, new CacheRequest());
            var pattern = (ScrollPattern)element.GetCachedPattern(ScrollPattern.Pattern);
            pattern.SetScrollPercent(horizontalPercent, verticalPercent);
        }

        protected override AutomationElement AssertPattern(string elementId, CacheRequest cacheRequest)
        {
            cacheRequest.Add(AutomationElement.IsScrollPatternAvailableProperty);
            cacheRequest.Add(ScrollPattern.Pattern);
            var element = finderService.GetElement(elementId, cacheRequest);
            if (!(bool)element.GetCachedPropertyValue(AutomationElement.IsScrollPatternAvailableProperty))
            {
                throw new InvalidArgument("Scroll pattern is not available for this element");
            }
            return element;
        }
    }
}
