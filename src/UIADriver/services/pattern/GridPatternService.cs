using UIADriver.dto.response;

namespace UIADriver.services.pattern
{
    public abstract class GridPatternService<T, U> : PatternService<T, U>
    {
        protected GridPatternService(ServiceProvider<T, U> serviceProvider) : base(serviceProvider) { }

        public abstract FindElementResponse GetItem(string elementId, int row, int column);
    }
}
