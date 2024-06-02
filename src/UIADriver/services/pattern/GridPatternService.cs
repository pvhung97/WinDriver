using UIADriver.dto.response;

namespace UIADriver.services.pattern
{
    public abstract class GridPatternService<T, U> : PatternService<T, U>
    {
        protected GridPatternService(ElementFinderService<T, U> finderService, ElementAttributeService<T> attributeService) : base(finderService, attributeService) { }

        public abstract int GetColumnCount(string elementId);
        public abstract int GetRowCount(string elementId);
        public abstract FindElementResponse GetItem(string elementId, int row, int column);
    }
}
