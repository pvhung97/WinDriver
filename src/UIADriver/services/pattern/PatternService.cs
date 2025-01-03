namespace UIADriver.services.pattern
{
    public abstract class PatternService<T, U>
    {
        protected ServiceProvider<T, U> serviceProvider;

        public PatternService(ServiceProvider<T, U> serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        protected abstract T AssertPattern(string elementId, U cacheRequest);
    }
}
