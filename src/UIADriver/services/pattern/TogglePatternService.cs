namespace UIADriver.services.pattern
{
    public abstract class TogglePatternService<T, U> : PatternService<T, U>
    {
        protected TogglePatternService(ServiceProvider<T, U> serviceProvider) : base(serviceProvider) { }

        public abstract void Toggle(string elementId);
    }
}
