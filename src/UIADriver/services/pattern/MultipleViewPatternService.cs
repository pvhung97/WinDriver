namespace UIADriver.services.pattern
{
    public abstract class MultipleViewPatternService<T, U> : PatternService<T, U>
    {
        protected MultipleViewPatternService(ServiceProvider<T, U> serviceProvider) : base(serviceProvider) { }

        public abstract List<int> GetSupportedViewIds(string elementId);
        public abstract string GetViewName(string elementId, int viewId);
        public abstract void SetCurrentView(string elementId, int viewId);
    }
}
