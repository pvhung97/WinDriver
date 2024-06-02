using UIADriver.dto.response;

namespace UIADriver.services.pattern
{
    public abstract class SpreadSheetPatternService<T, U> : PatternService<T, U>
    {
        protected SpreadSheetPatternService(ElementFinderService<T, U> finderService, ElementAttributeService<T> attributeService) : base(finderService, attributeService) { }

        public abstract FindElementResponse GetItemByName(string elementId, string name);
    }
}
