using UIADriver.dto.response;

namespace UIADriver.services.pattern
{
    public abstract class TablePatternService<T, U> : PatternService<T, U>
    {
        protected TablePatternService(ServiceProvider<T, U> serviceProvider) : base(serviceProvider) { }

        public abstract List<FindElementResponse> GetRowHeaders(string elementId);
        public abstract List<FindElementResponse> GetColumnHeaders(string elementId);
    }
}
