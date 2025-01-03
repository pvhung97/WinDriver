using UIADriver.dto.response;

namespace UIADriver.services.pattern
{
    public abstract class SpreadSheetPatternService<T, U> : PatternService<T, U>
    {
        protected SpreadSheetPatternService(ServiceProvider<T, U> serviceProvider) : base(serviceProvider) { }

        public abstract FindElementResponse GetItemByName(string elementId, string name);
    }
}
