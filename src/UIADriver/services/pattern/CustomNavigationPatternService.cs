using UIADriver.dto.response;

namespace UIADriver.services.pattern
{
    public abstract class CustomNavigationPatternService<T, U> : PatternService<T, U>
    {
        protected CustomNavigationPatternService(ElementFinderService<T, U> finderService, ElementAttributeService<T> attributeService) : base(finderService, attributeService) { }

        public abstract FindElementResponse Navigate(string elementId, string direction);
    }
}
