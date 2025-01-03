namespace UIADriver.services.pattern
{
    public abstract class ScrollItemPatternService<T, U> : PatternService<T, U>
    {
        protected ScrollItemPatternService(ServiceProvider<T, U> serviceProvider) : base(serviceProvider) { }

        public abstract void ScrollIntoView(string elementId);
    }
}
