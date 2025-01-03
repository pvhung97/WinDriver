using UIADriver.dto.response;

namespace UIADriver.services.pattern
{
    public abstract class TableItemPatternService<T, U> : PatternService<T, U>
    {
        protected TableItemPatternService(ServiceProvider<T, U> serviceProvider) : base(serviceProvider) { }

        public abstract List<FindElementResponse> GetRowHeaderItems(string elementId);
        public abstract List<FindElementResponse> GetColumnHeaderItems(string elementId);
    }
}
