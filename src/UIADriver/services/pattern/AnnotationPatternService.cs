using UIADriver.dto.response;

namespace UIADriver.services.pattern
{
    public abstract class AnnotationPatternService<T, U> : PatternService<T, U>
    {
        protected AnnotationPatternService(ElementFinderService<T, U> finderService, ElementAttributeService<T> attributeService) : base(finderService, attributeService) { }

        public abstract int GetAnnotationTypeId(string elementId);
        public abstract string GetAnnotationTypeName(string elementId);
        public abstract string GetAuthor(string elementId);
        public abstract string GetDateTime(string elementId);
        public abstract FindElementResponse GetTarget(string elementId);
    }
}
