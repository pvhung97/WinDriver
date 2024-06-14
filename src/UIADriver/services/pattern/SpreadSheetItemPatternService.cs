using UIADriver.dto.response;

namespace UIADriver.services.pattern
{
    public abstract class SpreadSheetItemPatternService<T, U> : PatternService<T, U>
    {
        protected SpreadSheetItemPatternService(ElementFinderService<T, U> finderService, ElementAttributeService<T> attributeService) : base(finderService, attributeService) { }

        public abstract List<FindElementResponse> GetAnnotationObjects(string elementId);
    }
}
