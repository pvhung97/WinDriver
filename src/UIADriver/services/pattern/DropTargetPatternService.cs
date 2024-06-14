namespace UIADriver.services.pattern
{
    public abstract class DropTargetPatternService<T, U> : PatternService<T, U>
    {
        protected DropTargetPatternService(ElementFinderService<T, U> finderService, ElementAttributeService<T> attributeService) : base(finderService, attributeService) { }

        public abstract List<string> GetDropTargetEffects(string elementId);
    }
}
