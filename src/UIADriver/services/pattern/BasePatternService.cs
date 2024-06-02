using UIADriver.dto.response;

namespace UIADriver.services.pattern
{
    public abstract class BasePatternService<T, U> : PatternService<T, U>
    {
        protected BasePatternService(ElementFinderService<T, U> finderService, ElementAttributeService<T> attributeService) : base(finderService, attributeService) { }

        public abstract void SetFocus(string elementId);
        public abstract FindElementResponse GetLabeledBy(string elementId);
        public abstract List<FindElementResponse> GetControllerFor(string elementId);
        public abstract List<FindElementResponse> GetDescribedBy(string elementId);
        public abstract List<FindElementResponse> GetFlowsTo(string elementId);
    }
}
