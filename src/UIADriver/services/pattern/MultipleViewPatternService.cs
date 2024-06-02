namespace UIADriver.services.pattern
{
    public abstract class MultipleViewPatternService<T, U> : PatternService<T, U>
    {
        protected MultipleViewPatternService(ElementFinderService<T, U> finderService, ElementAttributeService<T> attributeService) : base(finderService, attributeService) { }

        public abstract int GetCurrentViewId(string elementId);
        public abstract List<int> GetSupportedViewIds(string elementId);
        public abstract string GetViewName(string elementId, int viewId);
        public abstract void SetCurrentView(string elementId, int viewId);
    }
}
