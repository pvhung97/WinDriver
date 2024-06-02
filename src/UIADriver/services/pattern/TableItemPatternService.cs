using UIADriver.dto.response;

namespace UIADriver.services.pattern
{
    public abstract class TableItemPatternService<T, U> : PatternService<T, U>
    {
        protected TableItemPatternService(ElementFinderService<T, U> finderService, ElementAttributeService<T> attributeService) : base(finderService, attributeService) { }

        public abstract List<FindElementResponse> GetRowHeaderItems(string elementId);
        public abstract List<FindElementResponse> GetColumnHeaderItems(string elementId);
    }
}
