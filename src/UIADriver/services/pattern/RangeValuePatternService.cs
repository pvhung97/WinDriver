namespace UIADriver.services.pattern
{
    public abstract class RangeValuePatternService<T, U> : PatternService<T, U>
    {
        protected RangeValuePatternService(ElementFinderService<T, U> finderService, ElementAttributeService<T> attributeService) : base(finderService, attributeService) { }

        public abstract void SetValue(string elementId, double value);
    }
}
