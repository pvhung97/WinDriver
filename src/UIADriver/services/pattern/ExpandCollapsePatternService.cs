namespace UIADriver.services.pattern
{
    public abstract class ExpandCollapsePatternService<T, U> : PatternService<T, U>
    {
        public ExpandCollapsePatternService(ElementFinderService<T, U> finderService, ElementAttributeService<T> attributeService) : base(finderService, attributeService) { }
        public abstract void ExpandOrCollapseElement(string elementId, bool expand);
    }
}
