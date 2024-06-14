namespace UIADriver.services.pattern
{
    public abstract class DockPatternService<T, U> : PatternService<T, U>
    {
        public DockPatternService(ElementFinderService<T, U> finderService, ElementAttributeService<T> attributeService) : base (finderService, attributeService) { }

        public abstract void SetDockPosition(string elementId, string dockPosition);
        
    }
}
