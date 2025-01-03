using UIADriver.dto.response;

namespace UIADriver.services.pattern
{
    public abstract class SelectionPattern2Service<T, U> : PatternService<T, U>
    {
        protected SelectionPattern2Service(ServiceProvider<T, U> serviceProvider) : base(serviceProvider) { }

        public abstract FindElementResponse GetFirstSelectedItem(string elementId);
        public abstract FindElementResponse GetLastSelectedItem(string elementId);
        public abstract FindElementResponse GetCurrentSelectedItem(string elementId);
    }
}
