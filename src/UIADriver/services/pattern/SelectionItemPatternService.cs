using UIADriver.dto.response;

namespace UIADriver.services.pattern
{
    public abstract class SelectionItemPatternService<T, U> : PatternService<T, U>
    {
        protected SelectionItemPatternService(ElementFinderService<T, U> finderService, ElementAttributeService<T> attributeService) : base(finderService, attributeService) { }

        public abstract void Select(string elementId);
        public abstract void AddToSelection(string elementId);
        public abstract void RemoveFromSelection(string elementId);
        public abstract FindElementResponse GetSelectionContainer(string elementId);
    }
}
