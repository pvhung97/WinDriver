namespace UIADriver.services.pattern
{
    public abstract class InvokePatternService<T, U> : PatternService<T, U>
    {
        public InvokePatternService(ServiceProvider<T, U> serviceProvider) : base(serviceProvider) { }

        public abstract void Invoke(string elementId);
    }
}
