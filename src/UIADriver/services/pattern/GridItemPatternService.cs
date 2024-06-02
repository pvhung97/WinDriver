using UIADriver.dto.response;

namespace UIADriver.services.pattern
{
    public abstract class GridItemPatternService<T, U> : PatternService<T, U>
    {
        protected GridItemPatternService(ElementFinderService<T, U> finderService, ElementAttributeService<T> attributeService) : base(finderService, attributeService) { }

        public abstract int GetRow(string elementId);
        public abstract int GetColumn(string elementId);
        public abstract int GetRowSpan(string elementId);
        public abstract int GetColSpan(string elementId);
        public abstract FindElementResponse GetContainingGrid(string elementId);
    }
}
