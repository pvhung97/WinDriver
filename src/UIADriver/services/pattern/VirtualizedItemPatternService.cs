namespace UIADriver.services.pattern
{
    public abstract class VirtualizedItemPatternService<T, U> : PatternService<T, U>
    {
        protected VirtualizedItemPatternService(ElementFinderService<T, U> finderService, ElementAttributeService<T> attributeService) : base(finderService, attributeService) { }

        public abstract void Realize(string elementId);
    }
}
