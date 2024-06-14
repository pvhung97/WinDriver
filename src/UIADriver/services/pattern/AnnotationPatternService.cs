using UIADriver.dto.response;

namespace UIADriver.services.pattern
{
    public abstract class AnnotationPatternService<T, U> : PatternService<T, U>
    {
        protected AnnotationPatternService(ElementFinderService<T, U> finderService, ElementAttributeService<T> attributeService) : base(finderService, attributeService) { }

        public abstract FindElementResponse GetTarget(string elementId);
    }
}
