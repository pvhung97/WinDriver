using UIADriver.dto.response;

namespace UIADriver.services.pattern
{
    public abstract class SelectionItemPatternService<T, U> : PatternService<T, U>
    {
        protected SelectionItemPatternService(ServiceProvider<T, U> serviceProvider) : base(serviceProvider) { }

        public abstract void Select(string elementId);
        public abstract void AddToSelection(string elementId);
        public abstract void RemoveFromSelection(string elementId);
        public abstract FindElementResponse GetSelectionContainer(string elementId);
    }
}
