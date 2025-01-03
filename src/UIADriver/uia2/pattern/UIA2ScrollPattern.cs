using System.Windows.Automation;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia2.pattern
{
    public class UIA2ScrollPattern : ScrollPatternService<AutomationElement, CacheRequest>
    {
        public UIA2ScrollPattern(ServiceProvider<AutomationElement, CacheRequest> serviceProvider) : base(serviceProvider) { }

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
            var element = serviceProvider.GetElementFinderService().GetElement(elementId, cacheRequest);
            if (!(bool)element.GetCachedPropertyValue(AutomationElement.IsScrollPatternAvailableProperty))
            {
                throw new InvalidArgument("Scroll pattern is not available for this element");
            }
            return element;
        }
    }
}
