namespace UIADriver.services.pattern
{
    public abstract class ValuePatternService<T, U> : PatternService<T, U>
    {
        protected ValuePatternService(ServiceProvider<T, U> serviceProvider) : base(serviceProvider) { }

        public abstract void SetValue(string elementId, string value);
    }
}
