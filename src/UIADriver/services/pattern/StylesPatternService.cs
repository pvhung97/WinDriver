namespace UIADriver.services.pattern
{
    public abstract class StylesPatternService<T, U> : PatternService<T, U>
    {
        protected StylesPatternService(ElementFinderService<T, U> finderService, ElementAttributeService<T> attributeService) : base(finderService, attributeService) { }

        public abstract int GetStyleId(string elementId);
        public abstract string GetStyleName(string elementId);
        public abstract int GetFillColor(string elementId);
        public abstract string GetFillPatternStyle(string elementId);
        public abstract string GetShape(string elementId);
        public abstract int GetFillPatternColor(string elementId);
        public abstract string GetExtendedProperties(string elementId);
    }
}
