namespace UIADriver.services.pattern
{
    public abstract class RangeValuePatternService<T, U> : PatternService<T, U>
    {
        protected RangeValuePatternService(ServiceProvider<T, U> serviceProvider) : base(serviceProvider) { }

        public abstract void SetValue(string elementId, double value);
    }
}
