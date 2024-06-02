namespace UIADriver.services.pattern
{
    public abstract class TogglePatternService<T, U> : PatternService<T, U>
    {
        protected TogglePatternService(ElementFinderService<T, U> finderService, ElementAttributeService<T> attributeService) : base(finderService, attributeService) { }

        public abstract string GetToggleState(string elementId);
        public abstract void Toggle(string elementId);
    }
}
