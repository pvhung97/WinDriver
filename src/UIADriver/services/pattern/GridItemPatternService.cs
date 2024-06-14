using UIADriver.dto.response;

namespace UIADriver.services.pattern
{
    public abstract class GridItemPatternService<T, U> : PatternService<T, U>
    {
        protected GridItemPatternService(ElementFinderService<T, U> finderService, ElementAttributeService<T> attributeService) : base(finderService, attributeService) { }

        public abstract FindElementResponse GetContainingGrid(string elementId);
    }
}
