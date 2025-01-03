using UIADriver.dto.response;

namespace UIADriver.services.pattern
{
    public abstract class GridItemPatternService<T, U> : PatternService<T, U>
    {
        protected GridItemPatternService(ServiceProvider<T, U> serviceProvider) : base(serviceProvider) { }

        public abstract FindElementResponse GetContainingGrid(string elementId);
    }
}
