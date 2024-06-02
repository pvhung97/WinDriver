using UIADriver.dto.response;

namespace UIADriver.services.pattern
{
    public abstract class DragPatternService<T, U> : PatternService<T, U>
    {
        protected DragPatternService(ElementFinderService<T, U> finderService, ElementAttributeService<T> attributeService) : base(finderService, attributeService) { }

        public abstract bool IsGrabbed(string elementId);
        public abstract string GetDropEffect(string elementId);
        public abstract List<string> GetDropEffects(string elementId);
        public abstract List<FindElementResponse> GetGrabbedItems(string elementId);
    }
}
