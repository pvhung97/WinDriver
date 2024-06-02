namespace UIADriver.services.pattern
{
    public abstract class PatternService<T, U>
    {
        protected ElementFinderService<T, U> finderService;
        protected ElementAttributeService<T> attributeService;

        public PatternService(ElementFinderService<T, U> finderService, ElementAttributeService<T> attributeService)
        {
            this.finderService = finderService;
            this.attributeService = attributeService;
        }

        protected abstract T AssertPattern(string elementId, U cacheRequest);
    }
}
