namespace UIADriver.services.pattern
{
    public abstract class ScrollItemPatternService<T, U> : PatternService<T, U>
    {
        protected ScrollItemPatternService(ElementFinderService<T, U> finderService, ElementAttributeService<T> attributeService) : base(finderService, attributeService) { }

        public abstract void ScrollIntoView(string elementId);
    }
}
