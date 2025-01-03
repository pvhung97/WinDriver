using UIADriver.dto.response;

namespace UIADriver.services.pattern
{
    public abstract class DragPatternService<T, U> : PatternService<T, U>
    {
        protected DragPatternService(ServiceProvider<T, U> serviceProvider) : base(serviceProvider) { }

        public abstract List<string> GetDropEffects(string elementId);
        public abstract List<FindElementResponse> GetGrabbedItems(string elementId);
    }
}
