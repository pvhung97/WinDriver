using UIADriver.dto.response;

namespace UIADriver.services.pattern
{
    public abstract class SelectionPattern2Service<T, U> : PatternService<T, U>
    {
        protected SelectionPattern2Service(ElementFinderService<T, U> finderService, ElementAttributeService<T> attributeService) : base(finderService, attributeService) { }

        public abstract FindElementResponse GetFirstSelectedItem(string elementId);
        public abstract FindElementResponse GetLastSelectedItem(string elementId);
        public abstract FindElementResponse GetCurrentSelectedItem(string elementId);
        public abstract int GetItemCount(string elementId);
    }
}
