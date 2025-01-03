using UIADriver.dto.response;

namespace UIADriver.services.pattern
{
    public abstract class AnnotationPatternService<T, U> : PatternService<T, U>
    {
        protected AnnotationPatternService(ServiceProvider<T, U> serviceProvider) : base(serviceProvider) { }

        public abstract FindElementResponse GetTarget(string elementId);
    }
}
