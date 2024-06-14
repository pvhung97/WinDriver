using UIADriver.dto.response;

namespace UIADriver.services.pattern
{
    public abstract class DragPatternService<T, U> : PatternService<T, U>
    {
        protected DragPatternService(ElementFinderService<T, U> finderService, ElementAttributeService<T> attributeService) : base(finderService, attributeService) { }

        public abstract List<string> GetDropEffects(string elementId);
        public abstract List<FindElementResponse> GetGrabbedItems(string elementId);
    }
}
