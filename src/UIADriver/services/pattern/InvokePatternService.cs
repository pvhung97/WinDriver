namespace UIADriver.services.pattern
{
    public abstract class InvokePatternService<T, U> : PatternService<T, U>
    {
        public InvokePatternService(ElementFinderService<T, U> finderService, ElementAttributeService<T> attributeService) : base(finderService, attributeService) { }

        public abstract void Invoke(string elementId);
    }
}
