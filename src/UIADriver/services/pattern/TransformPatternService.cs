namespace UIADriver.services.pattern
{
    public abstract class TransformPatternService<T, U> : PatternService<T, U>
    {
        protected TransformPatternService(ElementFinderService<T, U> finderService, ElementAttributeService<T> attributeService) : base(finderService, attributeService) { }

        public abstract void Move(string elementId, double x, double y);
        public abstract void Resize(string elementId, double width, double height);
        public abstract void Rotate(string elementId, double degrees);
    }
}
