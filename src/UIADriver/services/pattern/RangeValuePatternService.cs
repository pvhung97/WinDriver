namespace UIADriver.services.pattern
{
    public abstract class RangeValuePatternService<T, U> : PatternService<T, U>
    {
        protected RangeValuePatternService(ElementFinderService<T, U> finderService, ElementAttributeService<T> attributeService) : base(finderService, attributeService) { }

        public abstract bool IsReadOnly(string elementId);
        public abstract double GetMinimum(string elementId);
        public abstract double GetMaximum(string elementId);
        public abstract double GetLargeChange(string elementId);
        public abstract double GetSmallChange(string elementId);
        public abstract double GetValue(string elementId);
        public abstract void SetValue(string elementId, double value);
    }
}
