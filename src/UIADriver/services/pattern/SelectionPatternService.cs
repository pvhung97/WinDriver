namespace UIADriver.services.pattern
{
    public abstract class SelectionPatternService<T, U> : PatternService<T, U>
    {
        protected SelectionPatternService(ElementFinderService<T, U> finderService, ElementAttributeService<T> attributeService) : base(finderService, attributeService) { }

        public abstract bool CanSelectMultiple(string elementId);
        public abstract bool IsSelectionRequired(string elementId);
    }
}
