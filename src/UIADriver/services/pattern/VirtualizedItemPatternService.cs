namespace UIADriver.services.pattern
{
    public abstract class VirtualizedItemPatternService<T, U> : PatternService<T, U>
    {
        protected VirtualizedItemPatternService(ServiceProvider<T, U> serviceProvider) : base(serviceProvider) { }

        public abstract void Realize(string elementId);
    }
}
