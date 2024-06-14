namespace UIADriver.services.pattern
{
    public abstract class ScrollPatternService<T, U> : PatternService<T, U>
    {
        protected ScrollPatternService(ElementFinderService<T, U> finderService, ElementAttributeService<T> attributeService) : base(finderService, attributeService) { }

        public abstract void Scroll(string elementId, string horizontalAmount, string verticalAmount);
        public abstract void SetScrollPercent(string elementId, double horizontalPercent, double verticalPercent);
    }
}
