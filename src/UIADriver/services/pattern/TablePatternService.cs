using UIADriver.dto.response;

namespace UIADriver.services.pattern
{
    public abstract class TablePatternService<T, U> : PatternService<T, U>
    {
        protected TablePatternService(ElementFinderService<T, U> finderService, ElementAttributeService<T> attributeService) : base(finderService, attributeService) { }

        public abstract List<FindElementResponse> GetRowHeaders(string elementId);
        public abstract List<FindElementResponse> GetColumnHeaders(string elementId);
        public abstract string GetRowOrColumnMajor(string elementId);
    }
}
