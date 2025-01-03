namespace UIADriver.services.pattern
{
    public abstract class ExpandCollapsePatternService<T, U> : PatternService<T, U>
    {
        public ExpandCollapsePatternService(ServiceProvider<T, U> serviceProvider) : base(serviceProvider) { }
        public abstract void ExpandOrCollapseElement(string elementId, bool expand);
    }
}
