using UIADriver.dto.response;

namespace UIADriver.services.pattern
{
    public abstract class CustomNavigationPatternService<T, U> : PatternService<T, U>
    {
        protected CustomNavigationPatternService(ServiceProvider<T, U> serviceProvider) : base(serviceProvider) { }

        public abstract FindElementResponse Navigate(string elementId, string direction);
    }
}
