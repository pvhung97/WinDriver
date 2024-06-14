namespace UIADriver.services.pattern
{
    public abstract class ValuePatternService<T, U> : PatternService<T, U>
    {
        protected ValuePatternService(ElementFinderService<T, U> finderService, ElementAttributeService<T> attributeService) : base(finderService, attributeService) { }

        public abstract void SetValue(string elementId, string value);
    }
}
