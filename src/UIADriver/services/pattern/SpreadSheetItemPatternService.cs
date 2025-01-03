using UIADriver.dto.response;

namespace UIADriver.services.pattern
{
    public abstract class SpreadSheetItemPatternService<T, U> : PatternService<T, U>
    {
        protected SpreadSheetItemPatternService(ServiceProvider<T, U> serviceProvider) : base(serviceProvider) { }

        public abstract List<FindElementResponse> GetAnnotationObjects(string elementId);
    }
}
